﻿using System;
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
            this.isWaitData = options.WaitForDataBeforeAllocatingBuffer;
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
        private bool isWaitData;

        protected volatile bool isDisposed = false;
        protected FilterPipeline<ITcpSession> filterPipeline;
        
        
        public abstract Task StartAsync();

        public abstract void Write(object obj);

        public abstract Task FlushAsync();
        
        protected void Run()
        {
            this.processingTask = this.SocketStartAsync(); 
            
            this.filterPipeline.OnTransportActive(this);
        }
        
        private async Task SocketStartAsync()
        {
            try
            {
                var rcvTask = DoReceive();
                var sndTask = DoSend();

                Console.WriteLine("111111111111111111111111111111111111");
                await rcvTask;
                Console.WriteLine("222222222222222222222222222222222222");
                await sndTask;
                Console.WriteLine("333333333333333333333333333333333333");
               
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
            catch(Exception ex)
            {
                if (ex is RemoteSocketClosedException)
                {
                    Console.WriteLine("远程socket已经被关闭");
                    this.DisposeAsync().NoAwait();
                }
                else if (!(SocketErrorHelper.IsSocketDisabledError(ex) || ex is ObjectDisposedException))
                {
                    this.filterPipeline.OnTransportException(this, ex);
                }
             
                Console.WriteLine("socket关闭成功");
            }
            finally
            {
                Console.WriteLine("DoReceive 结束");
                this.Input.Complete();
            }
        }
        
        private async Task ProcessReceives()
        {
            var input = Input;
            while (!this.isDisposed)
            {
                if (this.isWaitData)
                {
                    await this.socket.WaitAsync();
                }
                var buffer = input.GetMemory(this.minAllocBufferSize);
                var bytes = await this.socket.ReceiveAsync(ref buffer);
                
                if (bytes == 0)
                {
                    throw new RemoteSocketClosedException();
                }
                
                input.Advance(bytes);
                var result = await input.FlushAsync();
                if (result.IsCompleted || result.IsCanceled)
                {
                    break;
                }
            }
        }
        
        private async Task DoSend()
        {
            try
            {
                await ProcessSends();
            }
            catch (Exception ex)
            {
                if (!(ex is RemoteSocketClosedException || SocketErrorHelper.IsSocketDisabledError(ex) || ex is ObjectDisposedException))
                {
                    this.filterPipeline.OnTransportException(this, ex);
                    Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"+ex.GetType().Name);
                    await this.DisposeAsync();
                }
            }
            finally
            {
                Console.WriteLine("DoSend 结束");
                this.Output.Complete();
            }
        }
        
        private async Task ProcessSends()
        {
            var output = Output;
            while (!this.isDisposed)
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
                    var bytes = await this.socket.SendAsync(ref buffer);
                    if (bytes == 0)
                    {
                        throw new RemoteSocketClosedException();
                    }
                }

                output.AdvanceTo(end);
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

        public virtual async ValueTask DisposeAsync()
        {
            Console.WriteLine("关闭>>>"+this.isDisposed);
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                this.filterPipeline.OnTransportInactive(this);
                Console.WriteLine("ATcpSession.DisposeAsync>>1111111");
                this.Input.CancelPendingFlush();
                this.Output.CancelPendingRead();
                Console.WriteLine("ATcpSession.DisposeAsync>>2222222");
                await this.socket.DisposeAsync();
                Console.WriteLine("ATcpSession.DisposeAsync>>3333333");
                await this.processingTask;

                Console.WriteLine("ATcpSession.DisposeAsync>>4444444");
            }
        }
    }
}