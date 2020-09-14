using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Udp
{
    public partial class UdpContext : IUdpContext
    {
        public UdpContext(ChannelGroupDgramImpl<IUdpContext, IUdpChannel> channelGroup, MemoryPool<byte> memoryPool)
        {
            this.MemoryPool = memoryPool;
            this.channelGroup = channelGroup;
            this.onMemoryPushEvent = this.OnMemoryWritePush;
            
            this.senderPipeline = new UdpPipe(memoryPool, PipeScheduler.ThreadPool,this.OnSenderPipelineReceiveBuffer);
            this.receiverPipeline = new UdpPipe(memoryPool, PipeScheduler.ThreadPool, this.OnReceiverPipelineReceiveBuffer);
        }

        public string Id { get; }
        public EndPoint LoaclAddress { get; private set; }
        public EndPoint RemoteAddress { get; private set; }
        public MemoryPool<byte> MemoryPool { get; private set; }

        public BinaryOrder Order;

        private Socket socket;
        private UdpPipe receiverPipeline;
        private UdpPipe senderPipeline;
        private UdpSocketReceiver receiver;
        private UdpSocketSender sender;
        private WrappedDgramWriter.MemoryWritePushDelegate onMemoryPushEvent;

        private ChannelGroupDgramImpl<IUdpContext, IUdpChannel> channelGroup;
        
        public void Start(Socket socket)
        {
            this.socket = socket;
            this.LoaclAddress = this.socket.LocalEndPoint;
            this.RemoteAddress = this.socket.RemoteEndPoint;
            this.receiver = new UdpSocketReceiver(this.socket, PipeScheduler.ThreadPool);
            this.sender = new UdpSocketSender(this.socket, PipeScheduler.ThreadPool);
            
            

            ReaderMemoryPolledIOAsync().NoAwait();
            
            this.receiverPipeline.ReceiveAsync();
            this.senderPipeline.ReceiveAsync();

            this.channelGroup.OnChannelActive(this);
        }

        public Task WriteAsync(EndPoint remote, object o)
        {
            return this.SenderMemoryIOAsync(remote, o);
        }

        public Task WriteAsync(EndPoint remote, ref ReadOnlySequence<byte> sequence)
        {
            return this.SenderMemoryIOAsync(remote, ref sequence);
        }
        
        public Task CloseAsync()
        {
            this.channelGroup.OnChannelInactive(this);
            
            return Task.CompletedTask;
        }
        
        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}