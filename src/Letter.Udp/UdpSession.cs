using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public partial class UdpSession : IUdpSession
    {
        public UdpSession(BinaryOrder order, MemoryPool<byte> memoryPool, PipeScheduler scheduler, DgramChannelFilterGroup<IUdpSession, IUdpChannelFilter> filterGroup)
        {
            Console.WriteLine("session  创建");
            this.Id = IdGeneratorHelper.GetNextId();
            this.Scheduler = scheduler;
            this.MemoryPool = memoryPool;
            this.filterGroup = filterGroup;

            this.onMemoryWritePush = this.OnMemoryWritePush;
            this.senderPipeline = new UdpPipe(this.MemoryPool, this.Scheduler, this.OnSenderPipelineReceiveBuffer);
            this.receiverPipeline = new UdpPipe(this.MemoryPool, this.Scheduler, this.OnReceiverPipelineReceiveBuffer);
        }
        
        public string Id { get; private set; }
        public EndPoint LoaclAddress { get; private set; }

        public EndPoint RemoteAddress
        {
            get
            {
                throw new Exception("please use IUdpSession.RcvAddress or IUdpSession.SndAddress");
            }
        }

        public EndPoint RcvAddress { get; private set; }
        public EndPoint SndAddress { get; private set; }
        
        public PipeScheduler Scheduler { get; private set; }
        public MemoryPool<byte> MemoryPool { get; private set; }

        private Socket socket;
        
        private UdpSocketSender socketSender;
        private UdpSocketReceiver socketReceiver;
        
        private UdpPipe senderPipeline;
        private UdpPipe receiverPipeline;

        private BinaryOrder order;
        private DgramChannelFilterGroup<IUdpSession, IUdpChannelFilter> filterGroup;
        private WrappedDgramWriter.MemoryWritePushDelegate onMemoryWritePush;

        private Task memoryTask;

        private string name;

        protected long isClosed = 0;
        
        public void StartAsync(Socket socket, string name)
        {
            this.name = name;
            
            this.socket = socket;
            this.LoaclAddress = this.socket.LocalEndPoint;
            
            this.socketSender = new UdpSocketSender(this.socket, this.Scheduler);
            this.socketReceiver = new UdpSocketReceiver(this.socket, this.Scheduler);

            this.StartReceiveSenderPipelineBuffer();
            this.StartReceiveReceiverPipelineBuffer();
            
            this.memoryTask = this.SocketReceiveAsync();
            
            this.filterGroup.OnChannelActive(this);
        }
        
        public Task WriteAsync(EndPoint remoteAddress, object obj)
        {
            return this.WriteBufferAsync(remoteAddress, obj);
        }

        public Task WriteAsync(EndPoint remoteAddress, ref ReadOnlySequence<byte> sequence)
        {
            return this.WriteBufferAsync(remoteAddress, ref sequence);
        }
        
        public async Task CloseAsync()
        {
            if (System.Threading.Interlocked.Read(ref this.isClosed) == 1)
            {
                return;
            }

            System.Threading.Interlocked.Exchange(ref this.isClosed, 1);
            Console.WriteLine("close---close---close---close---close---close---close---");
            this.filterGroup.OnChannelInactive(this);

            this.Id = string.Empty;
            if (this.socket != null)
            {
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