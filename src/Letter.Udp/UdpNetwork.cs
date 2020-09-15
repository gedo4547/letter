using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Letter;

namespace Letter.Udp
{
    public class UdpNetwork : ADgramNetwork<IUdpSession, IUdpChannel>, IUdpNetwork
    {
        public UdpNetwork(UdpOptions options)
        {
            this.options = options;
        }

        private Socket socket;
        private UdpOptions options;

        public Task StartAsync(EndPoint bindAddress)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync(EndPoint bindAddress, EndPoint connectAddress)
        {
            this.CreateSocket(bindAddress.AddressFamily);
            return Task.CompletedTask;
        }

        private void CreateSocket(AddressFamily family)
        {
            this.socket = new Socket(family, SocketType.Dgram, ProtocolType.Udp);
            
        }


        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }
    }
}