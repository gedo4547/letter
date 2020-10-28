using System.Threading.Tasks;

using FilterFactory = Letter.ChannelFilterGroupFactory<Letter.Udp.IUdpSession, Letter.Udp.IUdpChannelFilter>;

namespace Letter.Udp
{
    sealed class UdpBootstrap : ABootstrap<UdpOptions, IUdpSession, IUdpChannel, IUdpChannelFilter>, IUdpBootstrap
    {
        protected override Task<IUdpChannel> ChannelFactory(UdpOptions options, FilterFactory filterFactory)
        {
            UdpChannel channel = new UdpChannel(options, filterFactory);
            return Task.FromResult((IUdpChannel) channel);
        }
    }
}