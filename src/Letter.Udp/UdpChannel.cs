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
            this.CreateSocket(bindAddress.AddressFamily);
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
        
        private void CreateSocket(AddressFamily family)
        {
            this.socket = new Socket(family, SocketType.Dgram, ProtocolType.Udp);
            
            if (this.options.RcvTimeout != null)
                this.socket.SettingRcvTimeout(this.options.RcvTimeout.Value);
            if (this.options.SndTimeout != null)
                this.socket.SettingSndTimeout(this.options.SndTimeout.Value);
            
            if (this.options.RcvBufferSize != null)
                this.socket.SettingRcvBufferSize(this.options.RcvBufferSize.Value);
            if (this.options.SndBufferSize != null)
                this.socket.SettingSndBufferSize(this.options.SndBufferSize.Value);
        }

        private void Run()
        {
            var filterGroup = this.groupFactory.CreateFilterGroup();
            var memoryPool = this.options.MemoryPoolFactory();
            PipeScheduler scheduler = this.options.SchedulerAllocator.Next();
            
            this.session = new UdpSession(
                this.socket, 
                this.options.Order, 
                memoryPool,
                scheduler, 
                filterGroup);
            
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