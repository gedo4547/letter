using System.Net;
using System.Threading.Tasks;
using Letter;

namespace Letter.Udp
{
    public interface IUdpNetwork : IDgramNetwork<IUdpSession, IUdpChannel>
    {
        Task StartAsync(EndPoint bindAddress);

        Task StartAsync(EndPoint bindAddress, EndPoint connectAddress);
    }
}