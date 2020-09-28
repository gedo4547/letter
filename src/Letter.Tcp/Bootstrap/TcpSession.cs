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

            this.Id = client.Id;
        }

      
        public string Id
        {
            get;
            private set;
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
        protected FilterGroup filterGroup;
        protected BinaryOrder order;
        
        protected PipeReader input;
        protected PipeWriter output;
        
        protected Task readBufferTask;
        protected object syncLock = new object();
        
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
                try
                {
                    Console.WriteLine("ReadSocketBufferAsync-----0>>" + Id + "        " + isDispose);
                    if (this.isDispose)
                    {
                        return;
                    }
                    
                    ReadResult result = await this.input.ReadAsync();
                    
                    if (result.IsCanceled || result.IsCompleted)
                    {
                        Console.WriteLine("ReadSocketBufferAsync-----1>>"+ Id);
                        break;
                    }
                    Console.WriteLine("ReadSocketBufferAsync-----2>>"+ Id);
                    var buffer = result.Buffer;
                    var reader = new WrappedStreamReader(this.input, ref buffer, ref order);
                    
                    this.filterGroup.OnChannelRead(this, ref reader);
                }
                catch (Exception e) when (e is ObjectDisposedException)
                {
                    Console.WriteLine("ReadSocketBufferAsync-----3>>"+ Id);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("ReadSocketBufferAsync-----4>>"+ Id);
                    this.filterGroup.OnChannelException(this, e);
                    break;
                }
            }
            
            Console.WriteLine("ReadSocketBufferAsync-----5>>"+ Id);
            try
            {
                this.input.Complete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Console.WriteLine("ReadSocketBufferAsync-----6>>"+ Id);
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
            await this.DisposeAsync();
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (this.isDispose)
            {
                return;
            }

            this.isDispose = true;
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Session.Colse111111111111>>"+ Id);
            await this.client.DisposeAsync();
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Session.Colse222222222222>>"+ Id);
            await this.readBufferTask;
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Session.Colse333333333333>>"+ Id);
            
            this.filterGroup.OnChannelInactive(this);
        }
    }
}