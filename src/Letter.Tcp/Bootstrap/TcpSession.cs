using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using FilterGroup = Letter.StreamChannelFilterGroup<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    class TcpSession : IInternalTcpSession
    {
        public TcpSession(ITcpClient client, IDuplexPipe transport, FilterGroup filterGroup)
        {
            this.client = client;
            this.filterGroup = filterGroup;
            this.transport = transport;

            this.client.AddClosedListener(this.OnClientClosed);
            this.client.AddExceptionListener(this.OnClientException);
        }

      
        public string Id
        {
            get { return this.client.Id; }
        }
        public EndPoint LoaclAddress
        {
            get { return this.client.LocalAddress; }
        }
        public EndPoint RemoteAddress
        {
            get { return this.client.RemoteAddress; }
        }
        public MemoryPool<byte> MemoryPool
        {
            get { return this.client.MemoryPool; }
        }
        public PipeScheduler Scheduler
        {
            get { return this.client.Scheduler; }
        }

        

        protected ITcpClient client;
        private FilterGroup filterGroup;
        private BinaryOrder order;
        
        private PipeReader input;
        private PipeWriter output;
        
        protected Task readBufferTask;
        private object syncLock = new object();
        
        protected IDuplexPipe transport;
        protected volatile bool isDispose = false;

        public virtual Task StartAsync()
        {
            this.input = this.transport.Input;
            this.output = this.transport.Output;
            
            this.readBufferTask = this.ReadSocketBufferAsync();

            this.filterGroup.OnChannelActive(this);
            return Task.CompletedTask;
        }

        private async Task ReadSocketBufferAsync()
        {
            while (true)
            {
                ReadResult result = await this.input.ReadAsync();
                if (result.IsCanceled || result.IsCompleted)
                {
                    break;
                }
                
                try
                {
                    var buffer = result.Buffer;
                    var reader = new WrappedStreamReader(this.input, ref buffer, ref order);

                    Console.WriteLine("isDispose>>>" + isDispose);
                    
                    if (this.isDispose)
                    {
                        break;
                    }
                    this.filterGroup.OnChannelRead(this, ref reader);
                }
                catch (Exception e)
                {
                   this.filterGroup.OnChannelException(this, e);
                   break;
                }
            }
        
            this.input.Complete();
        }

        public Task WriteAsync(object obj)
        {
            try
            {
                lock (syncLock)
                {
                    if (this.isDispose)
                    {
                        return Task.CompletedTask;
                    }
                    
                    WrappedStreamWriter writer = new WrappedStreamWriter(this.output, ref this.order);
                    this.filterGroup.OnChannelWrite(this, ref writer, obj);
                }
            }
            catch (Exception e)
            {
                this.filterGroup.OnChannelException(this, e);
                return Task.CompletedTask;
            }

            return this.WriteFlushAsync();
        }

        public Task WriteAsync(ref ReadOnlySequence<byte> sequence)
        {
            try
            {
                lock (syncLock)
                {
                    if (this.isDispose)
                    {
                        return Task.CompletedTask;
                    }
                    
                    WrappedStreamWriter writer = new WrappedStreamWriter(this.output, ref this.order);
                    this.filterGroup.OnChannelWrite(this, ref writer, ref sequence);
                }
            }
            catch (Exception e)
            {
                this.filterGroup.OnChannelException(this, e);
                return Task.CompletedTask;
            }

            return this.WriteFlushAsync();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task WriteFlushAsync()
        {
            FlushResult result = await this.output.FlushAsync();
            if (result.IsCompleted || result.IsCanceled)
            {
                this.output.Complete();
            }
        }
        
        private void OnClientException(Exception ex)
        {
            this.filterGroup.OnChannelException(this, ex);
        }

        private async void OnClientClosed(ITcpClient client)
        {
            Console.WriteLine("被动关闭session");
            await this.DisposeAsync();
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (this.isDispose)
            {
                return;
            }

            this.isDispose = true;
            
            this.filterGroup.OnChannelInactive(this);
            
            // this.transport.Input.Complete();
            // this.transport.Output.Complete();
            await this.client.DisposeAsync();
            Console.WriteLine("session>>>111111111111111111111");
          
            
            await this.readBufferTask;
        }
    }
}