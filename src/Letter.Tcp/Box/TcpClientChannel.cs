using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


using FilterGroupFactory = Letter.Bootstrap.ChannelFilterGroupFactory<Letter.Tcp.Box.ITcpSession, Letter.Tcp.Box.ITcpChannelFilter>;

namespace Letter.Tcp.Box
{
    class TcpClientChannel : ATcpChannel, ITcpClientChannel
    {
        public TcpClientChannel(TcpClientOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature) 
            : base(groupFactory, sslFeature)
        {
            this.options = options;
            this.memoryPool = this.options.MemoryPoolFactory();
            this.schedulerAllocator = this.options.SchedulerAllocator;
        }
        
        private Socket connectSocket;
        private TcpClientOptions options;
        private MemoryPool<byte> memoryPool;
        private SchedulerAllocator schedulerAllocator;

        private ATcpSession session;
        
        public EndPoint ConnectAddress { get; private set; }

        public async Task StartAsync(EndPoint address)
        {
            if (address is IPEndPoint)
                throw new NotSupportedException("The SocketConnectionFactory only supports IPEndPoints for now.");
            
            if (connectSocket == null)
                this.connectSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await this.connectSocket.ConnectAsync(address);
            this.ConnectAddress = this.connectSocket.LocalEndPoint;
            
            this.session = this.createSession(this.connectSocket, this.options, this.schedulerAllocator.Next(), this.memoryPool);
            await this.session.StartAsync();
        }
        
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}