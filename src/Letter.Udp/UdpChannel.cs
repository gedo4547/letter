using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using FilterGroupFactory = Letter.DgramChannelFilterGroupFactory<Letter.Udp.IUdpSession, Letter.Udp.IUdpChannelFilter>;

namespace Letter.Udp
{
    sealed class UdpChannel : IUdpChannel
    {
        public UdpChannel(UdpOptions options, FilterGroupFactory groupFactory)
        {
            this.options = options;
            this.groupFactory = groupFactory;
        }

        private Socket socket;
        private UdpSession session;
        
        private UdpOptions options;
        private FilterGroupFactory groupFactory;
        
        public EndPoint BindAddress { get; private set; }
        
        public async Task StartAsync(EndPoint bindAddress, EndPoint connectAddress)
        {
            this.Bind(bindAddress);

            await this.socket.ConnectAsync(connectAddress);
            
            this.Run();
        }
        
        public Task StartAsync(EndPoint bindAddress)
        {
            this.Bind(bindAddress);
            
            this.Run();
            
            return Task.CompletedTask;
        }


        private void Bind(EndPoint bindAddress)
        {
            this.socket = new Socket(bindAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                this.socket.Bind(bindAddress);
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                throw new AddressInUseException(e.Message, e);
            }

            this.BindAddress = this.socket.LocalEndPoint;
        }
        
        private void Run()
        {
            var filterGroup = this.groupFactory.CreateFilterGroup();
            var memoryPool = this.options.MemoryPoolFactory();
            PipeScheduler scheduler = this.options.SchedulerAllocator.Next();
            this.session = new UdpSession(this.socket, this.options, memoryPool, scheduler, filterGroup);
            
            this.session.StartAsync();
        }
        
        public async ValueTask DisposeAsync()
        {
            if (this.session != null)
            {
                await this.session.DisposeAsync();
                this.socket = null;
            }

            this.options = null;
            this.groupFactory = null;
        }
    }
}