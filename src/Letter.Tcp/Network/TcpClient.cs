using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpClient : ATcpNetwork<TcpClientOptions>, ITcpClient
    {
        public static readonly bool IsMacOS = OSPlatformHelper.IsOSX();
        public static readonly bool IsWindows = OSPlatformHelper.IsWindows();
        
        public TcpClient() : base(new TcpClientOptions())
        {
        }
        
        public string Id { get; private set; }
        public EndPoint LocalAddress { get; private set; }
        public EndPoint RemoteAddress { get; private set; }
        public IDuplexPipe Transport { get; private set; }
        public IDuplexPipe Application { get; private set; }
        public MemoryPool<byte> MemoryPool { get; private set; }

        private Action<Exception> onException;
        
        
        private Socket connectSocket;
        private TcpSocketReceiver receiver;
        private TcpSocketSender sender;
        private bool waitForData;
        private int minAllocBufferSize;
        
        private Task _processingTask;
        private readonly CancellationTokenSource _connectionClosedTokenSource = new CancellationTokenSource();
        
        private readonly object _shutdownLock = new object();
        private volatile bool _socketDisposed;
        private volatile Exception _shutdownReason;
        private readonly TaskCompletionSource _waitForConnectionClosedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        private bool _connectionClosed;

        
        public PipeWriter Input => Application.Output;
        public PipeReader Output => Application.Input;
        
        public void AddExceptionListener(Action<Exception> onException)
        {
            if (onException == null)
            {
                throw new ArgumentNullException(nameof(onException));
            }

            this.onException += onException;
        }

        public void Start(Socket socket, PipeScheduler scheduler)
        {
            if (socket is null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            this.connectSocket = socket;
            this.StartConfigurationConnect(scheduler);

            this.Run();
        }

        public async ValueTask ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
        {
            var ipEndPoint = endpoint as IPEndPoint;
            if (ipEndPoint is null)
                throw new NotSupportedException("The SocketConnectionFactory only supports IPEndPoints for now.");

            if (connectSocket == null)
            {
                this.connectSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                this.connectSocket.NoDelay = this.options.NoDelay;
            }
            
            await this.connectSocket.ConnectAsync(ipEndPoint);
            this.StartConfigurationConnect(PipeScheduler.ThreadPool);

            this.Run();
        }

        private void StartConfigurationConnect(PipeScheduler scheduler)
        {
            this.LocalAddress = this.connectSocket.LocalEndPoint;
            this.RemoteAddress = this.connectSocket.RemoteEndPoint;
            this.MemoryPool = this.options.MemoryPoolFactory();
            this.minAllocBufferSize = this.MemoryPool.MaxBufferSize / 2;
            
            this.waitForData = this.options.WaitForDataBeforeAllocatingBuffer;
            var awaiterScheduler = IsWindows ? scheduler : PipeScheduler.Inline;
            this.receiver = new TcpSocketReceiver(this.connectSocket, awaiterScheduler);
            this.sender = new TcpSocketSender(this.connectSocket, awaiterScheduler);
            long maxReadBufferSize = this.options.MaxReadBufferSize == null ? 0 : this.options.MaxReadBufferSize.Value;
            long maxWriteBufferSize = this.options.MaxWriteBufferSize == null ? 0 : this.options.MaxWriteBufferSize.Value;
            var inputOptions = new PipeOptions(this.MemoryPool, PipeScheduler.ThreadPool, scheduler, maxReadBufferSize, maxReadBufferSize / 2, useSynchronizationContext: false);
            var outputOptions = new PipeOptions(this.MemoryPool, scheduler, PipeScheduler.ThreadPool, maxWriteBufferSize, maxWriteBufferSize / 2, useSynchronizationContext: false);

            var pair = DuplexPipe.CreateConnectionPair(inputOptions, outputOptions);

            this.Transport = pair.Transport;
            this.Application = pair.Application;
        }

        private void Run()
        {
            this.Id = IdGeneratorHelper.GetNextId();
            
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

                receiver.Dispose();
                sender.Dispose();
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Unexpected exception in {nameof(TcpClient)}.{nameof(StartAsync)}.");
                sb.AppendLine(ex.ToString());
                // _trace.LogError(0, ex, $"Unexpected exception in {nameof(TcpSession)}.{nameof(StartAsync)}.");
                this.onException?.Invoke(new Exception(sb.ToString()));
            }
        }

        private async Task DoReceive()
        {
            Exception error = null;
            
            try
            {
                await ProcessReceives();
            }
            catch (SocketException ex) when (IsConnectionResetError(ex.SocketErrorCode))
            {
                error = new ConnectionResetException(ex.Message, ex);
                if (!_socketDisposed)
                {
                    this.onException?.Invoke(error);
                }
            }
            catch (Exception ex) when ((ex is SocketException socketEx && IsConnectionAbortError(socketEx.SocketErrorCode)) || ex is ObjectDisposedException)
            {
                // This exception should always be ignored because _shutdownReason should be set.
                error = ex;

                if (!_socketDisposed)
                {
                    // This is unexpected if the socket hasn't been disposed yet.
                    this.onException?.Invoke(error);
                }
            }
            catch (Exception ex)
            {
                // This is unexpected.
                error = ex;
                this.onException?.Invoke(error);
            }
            finally
            {
                // If Shutdown() has already bee called, assume that was the reason ProcessReceives() exited.
                Input.Complete(_shutdownReason ?? error);

                FireConnectionClosed();

                await _waitForConnectionClosedTcs.Task;
            }
        }
        
        private async Task ProcessReceives()
        {
            // Resolve `input` PipeWriter via the IDuplexPipe interface prior to loop start for performance.
            var input = Input;
            while (true)
            {
                if (waitForData)
                {
                    // Wait for data before allocating a buffer.
                    await receiver.WaitForDataAsync();
                }
                // Ensure we have some reasonable amount of buffer space
                var buffer = input.GetMemory(minAllocBufferSize);
                var bytesReceived = await receiver.ReceiveAsync(buffer);
                if (bytesReceived == 0)
                {
                    // FIN
                    this.onException?.Invoke(new ConnectionReadFinException());
                    break;
                }

                input.Advance(bytesReceived);
                var flushTask = input.FlushAsync();
                // var paused = !flushTask.IsCompleted;
                //
                // if (paused)
                // {
                //     // _trace.ConnectionPause(this.Id);
                // }

                var result = await flushTask;

                // if (paused)
                // {
                //     // _trace.ConnectionResume(this.Id);
                // }

                if (result.IsCompleted || result.IsCanceled)
                {
                    // Pipe consumer is shut down, do we stop writing
                    break;
                }
            }
        }
        
         private async Task DoSend()
        {
            Exception shutdownReason = null;
            Exception unexpectedError = null;

            try
            {
                await ProcessSends();
            }
            catch (SocketException ex) when (IsConnectionResetError(ex.SocketErrorCode))
            {
                shutdownReason = new ConnectionResetException(ex.Message, ex);
                this.onException?.Invoke(shutdownReason);
            }
            catch (Exception ex)
                when ((ex is SocketException socketEx && IsConnectionAbortError(socketEx.SocketErrorCode)) ||
                      ex is ObjectDisposedException)
            {
                // This should always be ignored since Shutdown() must have already been called by Abort().
                shutdownReason = ex;
                this.onException?.Invoke(shutdownReason);
            }
            catch (Exception ex)
            {
                shutdownReason = ex;
                unexpectedError = ex;
                this.onException?.Invoke(ex);
            }
            finally
            {
                Shutdown(shutdownReason);

                // Complete the output after disposing the socket
                Output.Complete(unexpectedError);

                // Cancel any pending flushes so that the input loop is un-paused
                Input.CancelPendingFlush();
            }
        }
        
        private async Task ProcessSends()
        {
            // Resolve `output` PipeReader via the IDuplexPipe interface prior to loop start for performance.
            var output = Output;
            while (true)
            {
                var result = await output.ReadAsync();

                if (result.IsCanceled)
                {
                    break;
                }

                var buffer = result.Buffer;

                var end = buffer.End;
                var isCompleted = result.IsCompleted;
                if (!buffer.IsEmpty)
                {
                    await sender.SendAsync(buffer);
                }

                output.AdvanceTo(end);

                if (isCompleted)
                {
                    break;
                }
            }
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

                // _trace.ConnectionWriteFin(this.Id, _shutdownReason.Message);

                try
                {
                    // Try to gracefully close the socket even for aborts to match libuv behavior.
                    connectSocket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                    // Ignore any errors from Socket.Shutdown() since we're tearing down the connection anyway.
                }

                connectSocket.Dispose();
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
                // _trace.LogError(0, ex, $"Unexpected exception in {nameof(TcpSession)}.{nameof(CancelConnectionClosedToken)}.");
            }
        }

        public override ValueTask CloseAsync(CancellationToken cancellationToken = default)
        {
            Shutdown(null);

            // Cancel ProcessSends loop after calling shutdown to ensure the correct _shutdownReason gets set.
            Output.CancelPendingRead();

            return default;
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            
            Transport.Input.Complete();
            Transport.Output.Complete();

            if (_processingTask != null)
            {
                await _processingTask;
            }

            _connectionClosedTokenSource.Dispose();
        }
        
        
        protected static bool IsConnectionResetError(SocketError errorCode)
        {
            // A connection reset can be reported as SocketError.ConnectionAborted on Windows.
            // ProtocolType can be removed once https://github.com/dotnet/corefx/issues/31927 is fixed.
            return errorCode == SocketError.ConnectionReset ||
                   errorCode == SocketError.Shutdown ||
                   (errorCode == SocketError.ConnectionAborted && IsWindows) ||
                   (errorCode == SocketError.ProtocolType && IsMacOS);
        }

        protected static bool IsConnectionAbortError(SocketError errorCode)
        {
            // Calling Dispose after ReceiveAsync can cause an "InvalidArgument" error on *nix.
            return errorCode == SocketError.OperationAborted ||
                   errorCode == SocketError.Interrupted ||
                   (errorCode == SocketError.InvalidArgument && !IsWindows);
        }
        
    }
}