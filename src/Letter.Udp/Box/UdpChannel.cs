using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using FilterFactory = Letter.Bootstrap.FilterGroupFactory<Letter.Udp.Box.IUdpSession, Letter.Udp.Box.IUdpChannelFilter>;

namespace Letter.Udp.Box
{
    sealed class UdpChannel : IUdpChannel
    {
        public UdpChannel(UdpOptions options, FilterFactory groupFactory)
        {
            this.options = options;
            this.groupFactory = groupFactory;
        }

        private Socket socket;
        private UdpOptions options;
        private FilterFactory groupFactory;
        
        public EndPoint BindAddress { get; private set; }
        
        
        public async Task StartAsync(EndPoint bindAddress, EndPoint connectAddress)
        {
            await this.StartAsync(bindAddress);

            await this.socket.ConnectAsync(connectAddress);
        }
        
        public Task StartAsync(EndPoint bindAddress)
        {
            this.Bind(bindAddress);
            
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

        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}