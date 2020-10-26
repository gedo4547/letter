using System.Threading.Tasks;
using Letter.Bootstrap;

using FilterFactory = Letter.Bootstrap.FilterGroupFactory<Letter.Udp.Box.IUdpSession, Letter.Udp.Box.IUdpChannelFilter>;

namespace Letter.Udp.Box
{
    sealed class UdpBootstrap : Letter.Bootstrap.ABootstrap<UdpOptions, IUdpSession, IUdpChannel, IUdpChannelFilter>, IUdpBootstrap
    {
        protected override Task<IUdpChannel> ChannelFactory(UdpOptions options, FilterFactory filterFactory)
        {
            throw new System.NotImplementedException();
        }
    }
}