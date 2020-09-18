using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using FilterGroup = Letter.DgramChannelFilterGroup<Letter.Udp.IUdpSession, Letter.Udp.IUdpChannelFilter>;


namespace Letter.Udp
{
    partial class UdpSession : IUdpSession
    {
        public UdpSession(Socket socket, BinaryOrder order, MemoryPool<byte> memoryPool, PipeScheduler scheduler, FilterGroup filterGroup)
        {
            this.Id = IdGeneratorHelper.GetNextId();
            
            this.socket = socket;
            this.Scheduler = scheduler;
            this.MemoryPool = memoryPool;
            this.filterGroup = filterGroup;
            this.LoaclAddress = this.socket.LocalEndPoint;
            
            this.socketSender = new UdpSocketSender(this.socket, this.Scheduler);
            this.socketReceiver = new UdpSocketReceiver(this.socket, this.Scheduler);

            this.onMemoryWritePush = this.OnMemoryWritePush;
            this.senderPipeline = new DgramPipeline(this.MemoryPool, this.Scheduler, this.OnSenderPipelineReceiveBuffer);
            this.receiverPipeline = new DgramPipeline(this.MemoryPool, this.Scheduler, this.OnReceiverPipelineReceiveBuffer);
        }
        
        public string Id { get; private set; }
        public EndPoint LoaclAddress { get; private set; }
        
        public EndPoint RcvAddress { get; private set; }
        public EndPoint SndAddress { get; private set; }
        
        public PipeScheduler Scheduler { get; private set; }
        public MemoryPool<byte> MemoryPool { get; private set; }
        
        public EndPoint RemoteAddress
        {
            get { throw new Exception("please use IUdpSession.RcvAddress or IUdpSession.SndAddress"); }
        }

        private Socket socket;
        
        private UdpSocketSender socketSender;
        private UdpSocketReceiver socketReceiver;
        
        private DgramPipeline senderPipeline;
        private DgramPipeline receiverPipeline;

        private BinaryOrder order;
        private FilterGroup filterGroup;
        private WrappedDgramWriter.MemoryWritePushDelegate onMemoryWritePush;

        private Task memoryTask;

        protected volatile bool isDisposed = false;
        
        private object writerSync = new object();
        
        public void StartAsync()
        {
            this.StartReceiveSenderPipelineBuffer();
            this.StartReceiveReceiverPipelineBuffer();
            
            this.memoryTask = this.SocketReceiveAsync();
            
            this.filterGroup.OnChannelActive(this);
        }
        
        public Task WriteAsync(EndPoint remoteAddress, object obj)
        {
            lock (writerSync)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException("UdpSession has been released");
                }
                
                return this.WriteBufferAsync(remoteAddress, obj);
            }
        }

        public Task WriteAsync(EndPoint remoteAddress, ref ReadOnlySequence<byte> sequence)
        {
            lock (writerSync)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException("UdpSession has been released");
                }
                
                return this.WriteBufferAsync(remoteAddress, ref sequence);
            }
        }
        
        public async ValueTask DisposeAsync()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.isDisposed = true;
            
            this.filterGroup.OnChannelInactive(this);

            this.Id = string.Empty;
            if (this.socket != null)
            {
                try
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                
                this.socket.Dispose();
                this.socket = null;
            }
            
            if (this.memoryTask != null)
            {
                await this.memoryTask;
                this.memoryTask = null;
            }

            if (this.socketSender != null)
            {
                this.socketSender.Dispose();
                this.socketSender = null;
            }

            if (this.socketReceiver != null)
            {
                this.socketReceiver.Dispose();
                this.socketReceiver = null;
            }

            if (this.senderPipeline != null)
            {
                this.senderPipeline.Dispose();
                this.senderPipeline = null;
            }

            if (this.receiverPipeline != null)
            {
                this.receiverPipeline.Dispose();
                this.receiverPipeline = null;
            }

            if (this.filterGroup != null)
            {
                await this.filterGroup.DisposeAsync();
                this.filterGroup = null;
            }

            if (this.onMemoryWritePush != null)
            {
                this.onMemoryWritePush = null;
            }

            if (this.LoaclAddress != null)
            {
                this.LoaclAddress = null;
            }

            if (this.RcvAddress != null)
            {
                this.RcvAddress = null;
            }

            if (this.SndAddress != null)
            {
                this.SndAddress = null;
            }

            if (this.Scheduler != null)
            {
                this.Scheduler = null;
            }

            if (this.MemoryPool != null)
            {
                this.MemoryPool.Dispose();
                this.MemoryPool = null;
            }
        }
    }
}