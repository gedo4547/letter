using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public BinaryOrder Order
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public EndPoint LocalAddress
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public EndPoint RemoteAddress
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public MemoryPool<byte> MemoryPool
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public PipeScheduler Scheduler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        protected IWrappedDuplexPipe Transport
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        protected IWrappedDuplexPipe Application
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        private PipeWriter Input
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.Application.Output; }
        }

        private PipeReader Output
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.Application.Input;}
        }


        private TcpSocket socket;
        protected FilterPipeline<ITcpSession> filterPipeline;
        
        private Task processingTask;
        private int minAllocBufferSize;
        
        public abstract Task StartAsync();

        public abstract Task WriteAsync(object obj);
        
        protected void Run()
        {
            this.processingTask = SocketStartAsync(); 
        }
        
        private async Task SocketStartAsync()
        {
            try
            {
                var receiveTask = DoReceive();
                var sendTask = DoSend();

                await receiveTask;
                await sendTask;

                await this.socket.DisposeAsync();
            }
            catch (Exception ex)
            {
                this.filterPipeline.OnTransportException(this, ex);
            }
        }

        private async Task DoReceive()
        {            
            try
            {
                await ProcessReceives();
            }
            catch (Exception ex)
            {
                if (SocketErrorHelper.IsSocketDisabledError(ex) || ex is RemoteSocketClosedException)
                {
                    await this.DisposeAsync();
                }
                else
                {
                    this.filterPipeline.OnTransportException(this, ex);
                }
            }
        }
        
        private async Task ProcessReceives()
        {
            var input = Input;
            while (true)
            {
                var buffer = input.GetMemory(minAllocBufferSize);
                var bytesReceived = await this.socket.ReceiveAsync(ref buffer);
                
                if (bytesReceived == 0) 
                    throw new RemoteSocketClosedException();

                input.Advance(bytesReceived);
                var result = await input.FlushAsync();
                if (result.IsCompleted || result.IsCanceled) break;
            }
        }
        
        private async Task DoSend()
        {
            try
            {
                await ProcessSends();
            }
            catch(Exception ex)
            {
                if (!(SocketErrorHelper.IsSocketDisabledError(ex) || ex is RemoteSocketClosedException))
                {
                    this.filterPipeline.OnTransportException(this, ex);
                }
            }
        }
        
        private async Task ProcessSends()
        {
            var output = Output;
            while (true)
            {
                var result = await output.ReadAsync();
                if (result.IsCanceled) break;
                var buffer = result.Buffer;
                var end = buffer.End;
                var isCompleted = result.IsCompleted;
                if (!buffer.IsEmpty)
                {
                    var bytesReceived = await this.socket.SendAsync(ref buffer);
                    if (bytesReceived == 0)
                        throw new RemoteSocketClosedException();
                }

                output.AdvanceTo(end);
                if (isCompleted) break;
            }
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

        public async virtual ValueTask DisposeAsync()
        {
            await this.socket.DisposeAsync();
        }
    }
}