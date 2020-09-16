using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public class UdpChannel : IUdpChannel
    {
        public UdpChannel(UdpOptions options, DgramChannelFilterGroupFactory<IUdpSession, IUdpChannelFilter> groupFactory)
        {
            this.options = options;
            this.groupFactory = groupFactory;
        }

        private Socket socket;
        
        private UdpOptions options;
        private DgramChannelFilterGroupFactory<IUdpSession, IUdpChannelFilter> groupFactory;

        public Task StartAsync(EndPoint bindAddress)
        {
            this.CreateSocket(bindAddress.AddressFamily);
            return Task.CompletedTask;
        }

        public Task StartAsync(EndPoint bindAddress, EndPoint connectAddress)
        {
            this.CreateSocket(bindAddress.AddressFamily);
            return Task.CompletedTask;
        }

        private Socket CreateSocket(AddressFamily family)
        {
            this.socket = new Socket(family, SocketType.Dgram, ProtocolType.Udp);


            return this.socket;
        }

        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}