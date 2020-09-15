using System.Net;
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

        private UdpOptions options;

        public Task StartAsync(EndPoint bindAddress)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync(EndPoint bindAddress, EndPoint connectAddress)
        {
            return Task.CompletedTask;
        }

        private void Create()
        {
            
        }


        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }
    }
}