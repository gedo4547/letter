using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Letter.Tcp
{
    partial class TcpSession : ITcpSession
    {
        private static readonly bool IsMacOS = OSPlatformHelper.IsOSX();
        private static readonly bool IsWindows = OSPlatformHelper.IsWindows();
        
        public TcpSession(
            MemoryPool<byte> memoryPool,
            PipeScheduler scheduler,
            ISocketsTrace trace,
            long? maxReadBufferSize = null,
            long? maxWriteBufferSize = null,
            bool waitForData = true)
        {
            
            MemoryPool = memoryPool;
            _trace = trace;
            _waitForData = waitForData;

            MinAllocBufferSize = MemoryPool.MaxBufferSize / 2;
            
            LocalEndPoint = _socket.LocalEndPoint;
            RemoteEndPoint = _socket.RemoteEndPoint;

            ConnectionClosed = _connectionClosedTokenSource.Token;

            // On *nix platforms, Sockets already dispatches to the ThreadPool.
            // Yes, the IOQueues are still used for the PipeSchedulers. This is intentional.
            // https://github.com/aspnet/KestrelHttpServer/issues/2573
            var awaiterScheduler = IsWindows ? scheduler : PipeScheduler.Inline;

            _receiver = new TcpSocketReceiver(_socket, awaiterScheduler);
            _sender = new TcpSocketSender(_socket, awaiterScheduler);

            maxReadBufferSize ??= 0;
            maxWriteBufferSize ??= 0;

            var inputOptions = new PipeOptions(MemoryPool, PipeScheduler.ThreadPool, scheduler, maxReadBufferSize.Value, maxReadBufferSize.Value / 2, useSynchronizationContext: false);
            var outputOptions = new PipeOptions(MemoryPool, scheduler, PipeScheduler.ThreadPool, maxWriteBufferSize.Value, maxWriteBufferSize.Value / 2, useSynchronizationContext: false);

            var pair = DuplexPipe.CreateConnectionPair(inputOptions, outputOptions);

            // Set the transport and connection id
            Transport = pair.Transport;
            Application = pair.Application;
        }

        private string id;
        public string Id
        {
            get
            {
                if (id == null)
                {
                    id = IdGeneratorHelper.GetNextId();
                }

                return id;
            }
        }
        
        public EndPoint LocalEndPoint { get; private set; }
        public EndPoint RemoteEndPoint { get; private set; }
        public MemoryPool<byte> MemoryPool { get; private set; }
        public IDuplexPipe Transport { get; private set; }
        public IDuplexPipe Application { get; private set; }
        public CancellationToken ConnectionClosed { get; private set; }


        private int MinAllocBufferSize;
        
        private Socket _socket;
        private readonly ISocketsTrace _trace;
        private readonly TcpSocketReceiver _receiver;
        private readonly TcpSocketSender _sender;
        private readonly CancellationTokenSource _connectionClosedTokenSource = new CancellationTokenSource();
        
        private readonly object _shutdownLock = new object();
        private volatile bool _socketDisposed;
        private volatile Exception _shutdownReason;
        private Task _processingTask;
        private readonly TaskCompletionSource _waitForConnectionClosedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        private bool _connectionClosed;
        private readonly bool _waitForData;
        
        public PipeWriter Input => Application.Output;
        public PipeReader Output => Application.Input;
        
        public void Start(Socket socket)
        {
            _socket = socket;
            _processingTask = StartAsync();
        }

        private async Task StartAsync()
        {
            try
            {
                // Spawn send and receive logic
                var receiveTask = DoReceive();
                var sendTask = DoSend();

                // Now wait for both to complete
                await receiveTask;
                await sendTask;

                _receiver.Dispose();
                _sender.Dispose();
            }
            catch (Exception ex)
            {
                _trace.LogError(0, ex, $"Unexpected exception in {nameof(TcpSession)}.{nameof(StartAsync)}.");
            }
        }
        
        public void Abort(ConnectionAbortedException abortReason)
        {
            // Try to gracefully close the socket to match libuv behavior.
            Shutdown(abortReason);

            // Cancel ProcessSends loop after calling shutdown to ensure the correct _shutdownReason gets set.
            Output.CancelPendingRead();
        }

        // Only called after connection middleware is complete which means the ConnectionClosed token has fired.
        public async ValueTask DisposeAsync()
        {
            Transport.Input.Complete();
            Transport.Output.Complete();

            if (_processingTask != null)
            {
                await _processingTask;
            }

            _connectionClosedTokenSource.Dispose();
        }
        
        private void Shutdown(Exception shutdownReason)
        {
            lock (_shutdownLock)
            {
                if (_socketDisposed)
                {
                    return;
                }

                // Make sure to close the connection only after the _aborted flag is set.
                // Without this, the RequestsCanBeAbortedMidRead test will sometimes fail when
                // a BadHttpRequestException is thrown instead of a TaskCanceledException.
                _socketDisposed = true;

                // shutdownReason should only be null if the output was completed gracefully, so no one should ever
                // ever observe the nondescript ConnectionAbortedException except for connection middleware attempting
                // to half close the connection which is currently unsupported.
                _shutdownReason = shutdownReason ?? new ConnectionAbortedException("The Socket transport's send loop completed gracefully.");

                _trace.ConnectionWriteFin(this.Id, _shutdownReason.Message);

                try
                {
                    // Try to gracefully close the socket even for aborts to match libuv behavior.
                    _socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                    // Ignore any errors from Socket.Shutdown() since we're tearing down the connection anyway.
                }

                _socket.Dispose();
            }
        }
        
        private void FireConnectionClosed()
        {
            // Guard against scheduling this multiple times
            if (_connectionClosed)
            {
                return;
            }

            _connectionClosed = true;

            ThreadPool.UnsafeQueueUserWorkItem(state =>
            {
                state.CancelConnectionClosedToken();

                state._waitForConnectionClosedTcs.TrySetResult();
            },
            this,
            preferLocal: false);
        }

        private void CancelConnectionClosedToken()
        {
            try
            {
                _connectionClosedTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                _trace.LogError(0, ex, $"Unexpected exception in {nameof(TcpSession)}.{nameof(CancelConnectionClosedToken)}.");
            }
        }
        
        public Task CloseAsync()
        {
            throw new System.NotImplementedException();
        }

        private static bool IsConnectionResetError(SocketError errorCode)
        {
            // A connection reset can be reported as SocketError.ConnectionAborted on Windows.
            // ProtocolType can be removed once https://github.com/dotnet/corefx/issues/31927 is fixed.
            return errorCode == SocketError.ConnectionReset ||
                   errorCode == SocketError.Shutdown ||
                   (errorCode == SocketError.ConnectionAborted && IsWindows) ||
                   (errorCode == SocketError.ProtocolType && IsMacOS);
        }

        private static bool IsConnectionAbortError(SocketError errorCode)
        {
            // Calling Dispose after ReceiveAsync can cause an "InvalidArgument" error on *nix.
            return errorCode == SocketError.OperationAborted ||
                   errorCode == SocketError.Interrupted ||
                   (errorCode == SocketError.InvalidArgument && !IsWindows);
        }
    }
}