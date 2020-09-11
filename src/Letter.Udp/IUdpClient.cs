using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public interface IUdpClient : IDgramNetwork<UdpOptions, IUdpContext>
    {
        Task StartAsync(EndPoint bindLocalAddress);

        Task StartAsync(EndPoint bindLocalAddress, EndPoint connectRemoteAddress);
    }
}