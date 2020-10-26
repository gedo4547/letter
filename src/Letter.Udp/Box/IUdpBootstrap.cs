using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp.Box
{
    public interface IUdpBootstrap : Letter.Bootstrap.IBootstrap<UdpOptions, IUdpSession, IUdpChannel, IUdpChannelFilter>
    {
        
    }
}