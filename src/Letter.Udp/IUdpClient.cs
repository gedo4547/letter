using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public interface IUdpClient : IDgramNetwork<UdpOptions, IUdpContext, IUdpChannel>
    {
        Task StartAsync(EndPoint bindAddress);

        Task StartAsync(EndPoint bindAddress, EndPoint connectAddress);
    }
}