using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    abstract class ATcpSession : ITcpSession
    {
        public ATcpSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool, FilterPipeline<ITcpSession> filterPipeline)
        {
            this.Id = IdGeneratorHelper.GetNextId();
            this.Order = options.Order;
            this.MemoryPool = pool;
            this.Scheduler = scheduler;
            this.MemoryPool = pool;
            this.Scheduler = scheduler;
            this.minAllocBufferSize = this.MemoryPool.MaxBufferSize / 2;
            
            this.filterPipeline = filterPipeline;
            this.socket = new TcpSocket(socket, scheduler);
            this.LocalAddress = this.socket.BindAddress;
            this.RemoteAddress = this.socket.RemoteAddress;

            this.SettingSocket(this.socket, options);
            this.SettingPipeline(options.MaxPipelineReadBufferSize, options.MaxPipelineWriteBufferSize);
        }

        public string Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
        }

        public BinaryOrder Order
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
        }

        public EndPoint LocalAddress
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
        }

        public EndPoint RemoteAddress
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
        }

        public MemoryPool<byte> MemoryPool
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
        }

        public PipeScheduler Scheduler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
        }

        protected IWrappedDuplexPipe Transport
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        }

        protected IWrappedDuplexPipe Application
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        }

        private PipeWriter Input
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.Application.Output; }
        }

        private PipeReader Output
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.Application.Input; }
        }
        
        private TcpSocket socket;
        private Task processingTask;
        private int minAllocBufferSize;
        
        protected volatile bool isDisposed = false;
        protected FilterPipeline<ITcpSession> filterPipeline;
        
        
        public abstract Task StartAsync();

        public abstract void Write(object obj);

        public abstract ValueTask FlushAsync();
        
        protected void Run()
        {
            this.processingTask = this.SocketStartAsync();

            try
            {
                this.filterPipeline.OnTransportActive(this);
            }
            catch (Exception e)
            {
                this.DeliverException(e);
            }
        }
        
        private async Task SocketStartAsync()
        {
            try
            {
                var rcvTask = this.DoRcvAsync();
                var sndTask = this.DoSndAsync();

                await rcvTask;
                await sndTask;
#if DEBUG
                Logger.Info("tcpSession application SocketStartAsync End");
#endif
            }
            catch (Exception ex)
            {
                this.DeliverException(ex);
            }
        }

        private async Task DoRcvAsync()
        {
            try
            {
                await this.ProcessRcvAsync();
            }
            catch(ObjectDisposedException)
            {
            }
            finally
            {
                this.Input.Complete();
#if DEBUG
                Logger.Info("tcpSession application DoRcvAsync End");
#endif
            }
        }
        
        private async Task ProcessRcvAsync()
        {
            var input = Input;
            while (true)
            {
                await this.socket.WaitAsync();
                var buffer = input.GetMemory(this.minAllocBufferSize);
                var socketResult = await this.socket.ReceiveAsync(buffer);
                
                if (socketResult.error != SocketError.Success)
                {
                    if (!SocketErrorHelper.IsSocketDisabledError(socketResult.error))
                        this.DeliverException(new SocketException((int) socketResult.error));
                    else
                        this.CloseAsync().NoAwait();
                    
                    break;
                }

                if (socketResult.bytesTransferred == 0)
                {
                    this.CloseAsync().NoAwait();
                    break;
                }
                
                input.Advance(socketResult.bytesTransferred);
                var result = await input.FlushAsync();
                if (result.IsCompleted || result.IsCanceled)
                {
                    break;
                }
            }
        }
        
        private async Task DoSndAsync()
        {
            try
            {
                await this.ProcessSndAsync();
            }
            catch (ObjectDisposedException)
            {
            }
            finally
            {
                this.Output.Complete();
#if DEBUG
                Logger.Info("tcpSession application DoSndAsync End");
#endif
            }
        }
        
        private async Task ProcessSndAsync()
        {
            var output = Output;
            while (true)
            {
                var result = await output.ReadAsync();
                if (result.IsCanceled || result.IsCompleted)
                {
                    break;
                }

                var buffer = result.Buffer;
                var end = buffer.End;
                if (!buffer.IsEmpty)
                {
                    var socketResult = await this.socket.SendAsync(buffer);
                    
                    if (socketResult.error != SocketError.Success)
                    {
                        if (!SocketErrorHelper.IsSocketDisabledError(socketResult.error))
                            this.DeliverException(new SocketException((int) socketResult.error));
                        break;
                    }

                    if (socketResult.bytesTransferred == 0) break;
                }

                output.AdvanceTo(end);
            }
        }
        
        protected void DeliverException(Exception ex)
        {
            this.filterPipeline.OnTransportException(this, ex);
            this.CloseAsync().NoAwait();
        }
        
        private void SettingSocket(TcpSocket socket, ATcpOptions options)
        {
            this.socket.SettingKeepAlive(options.KeepAlive);
            this.socket.SettingLingerState(options.LingerOption);
            this.socket.SettingNoDelay(options.NoDelay);

            if (options.RcvTimeout != null)
                this.socket.SettingRcvTimeout(options.RcvTimeout.Value);
            if (options.SndTimeout != null)
                this.socket.SettingSndTimeout(options.SndTimeout.Value);
            if (options.RcvBufferSize != null)
                this.socket.SettingRcvBufferSize(options.RcvBufferSize.Value);
            if (options.SndBufferSize != null)
                this.socket.SettingSndBufferSize(options.SndBufferSize.Value);
        }

        private void SettingPipeline(long? MaxPipelineReadBufferSize, long? MaxPipelineWriteBufferSize)
        {
            long maxReadBufferSize = MaxPipelineReadBufferSize == null ? 0 : MaxPipelineReadBufferSize.Value;
            long maxWriteBufferSize = MaxPipelineWriteBufferSize == null ? 0 : MaxPipelineWriteBufferSize.Value;
            var inputOptions = new PipeOptions(this.MemoryPool, PipeScheduler.ThreadPool, Scheduler, maxReadBufferSize, maxReadBufferSize / 2, useSynchronizationContext: false);
            var outputOptions = new PipeOptions(this.MemoryPool, Scheduler, PipeScheduler.ThreadPool, maxWriteBufferSize, maxWriteBufferSize / 2, useSynchronizationContext: false);

            var pair = DuplexPipe.CreateConnectionPair(inputOptions, outputOptions);
            this.Transport = pair.Transport;
            this.Application = pair.Application;
        }

        public virtual async Task CloseAsync()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                this.filterPipeline.OnTransportInactive(this);
                
                this.Input.CancelPendingFlush();
                this.Output.CancelPendingRead();
                await this.socket.DisposeAsync();
                await this.processingTask;

#if DEBUG
                Logger.Info("tcpSession application out");
#endif
            }
        }
    }
}