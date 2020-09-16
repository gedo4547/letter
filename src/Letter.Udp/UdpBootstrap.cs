using System.Threading.Tasks;

namespace Letter.Udp
{
    public class UdpBootstrap : ADgramBootstrap<UdpOptions, IUdpSession, IUdpChannelFilter, IUdpChannel>, IUdpBootstrap
    {
        protected override Task<IUdpChannel> ChannelFactory(UdpOptions options, DgramChannelFilterGroupFactory<IUdpSession, IUdpChannelFilter> groupFactory)
        {
            IUdpChannel channel = new UdpChannel(options, groupFactory);

            return Task.FromResult(channel);
        }
    }
}