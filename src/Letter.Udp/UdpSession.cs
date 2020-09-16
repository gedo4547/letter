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
            get { throw new Exception("Udp does not support access to remote addresses");}
        }
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
        
        
        public void StartAsync(Socket socket)
        {
            this.socket = socket;
            this.LoaclAddress = this.socket.LocalEndPoint;
            
            this.socketSender = new UdpSocketSender(this.socket, this.Scheduler);
            this.socketReceiver = new UdpSocketReceiver(this.socket, this.Scheduler);

            this.SenderStartReceiveBuffer();
            this.ReceiverStartReceiveBuffer();
            
            this.memoryTask = this.ReaderMemoryPolledIOAsync();
            
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
        
        public Task CloseAsync()
        {
            this.Id = string.Empty;




            return Task.CompletedTask;
        }
    }
}