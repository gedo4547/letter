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
            this.Transport = transport;

            this.client.AddClosedListener(this.OnClientClosed);
            this.client.AddExceptionListener(this.OnClientException);
        }

      
        public string Id { get { return this.client.Id; } }

        public EndPoint LoaclAddress { get { return this.client.LocalAddress; } }

        public EndPoint RemoteAddress {get{return this.client.RemoteAddress;}}

        public MemoryPool<byte> MemoryPool {get{return this.client.MemoryPool;}}

        public PipeScheduler Scheduler {get{return this.client.Scheduler;}}

        private IDuplexPipe Transport
        {
            get;
            set;
        }

        private ITcpClient client;
        private FilterGroup filterGroup;
        private BinaryOrder order;

        private PipeReader input;
        private PipeWriter output;

        private Task readBufferTask;
        private object syncLock = new object();
        private volatile bool isDispose;

        public virtual Task StartAsync()
        {
            this.input = this.Transport.Input;
            this.output = this.Transport.Output;
            
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

        private void OnClientClosed(ITcpClient client)
        {
            this.DisposeAsync();
        }

        public virtual ValueTask DisposeAsync()
        {
            Console.WriteLine("DisposeAsyncDisposeAsyncDisposeAsync");
            this.filterGroup.OnChannelInactive(this);

            return default;
        }
    }
}