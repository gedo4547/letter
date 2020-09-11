using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Udp
{
    public class UdpContext : IUdpContext
    {
        public UdpContext(ChannelGroupDgramImpl<IUdpContext> channelGroup, MemoryPool<byte> memoryPool)
        {
            this.MemoryPool = memoryPool;
            this.channelGroup = channelGroup;
            this.socketPipeline = new UdpPipe(memoryPool, PipeScheduler.ThreadPool, this.OnReceiveBuffer);
        }

        public string Id
        {
            get;
        }

        public EndPoint LoaclAddress
        {
            get; private set;
        }

        public EndPoint RemoteAddress
        {
            get; private set;
        }

        public MemoryPool<byte> MemoryPool
        {
            get; private set;
        }

        private Socket socket;
        private UdpPipe socketPipeline;
        private ChannelGroupDgramImpl<IUdpContext> channelGroup;
        
        public void Start(Socket socket)
        {
            this.socket = socket;

            this.LoaclAddress = this.socket.LocalEndPoint;
            this.RemoteAddress = this.socket.RemoteEndPoint;

            this.channelGroup.OnChannelActive(this);
        }
        
        private void OnReceiveBuffer(IUdpPipeReader reader)
        {
            throw new System.NotImplementedException();
        }
        
        public Task WriteAsync(EndPoint remote, object o)
        {
            throw new System.NotImplementedException();
        }

        public Task WriteAsync(EndPoint remote, byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
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