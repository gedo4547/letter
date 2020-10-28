using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public interface IUdpBootstrap : IBootstrap<UdpOptions, IUdpSession, IUdpChannel, IUdpChannelFilter>
    {
        
    }
}