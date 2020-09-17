using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public interface IUdpChannel : IChannel
    {
        Task StartAsync(EndPoint bindAddress);

        Task StartAsync(EndPoint bindAddress, EndPoint connectAddress);
    }
}