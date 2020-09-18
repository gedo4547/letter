using System.Threading.Tasks;

using FilterGroupFactory = Letter.DgramChannelFilterGroupFactory<Letter.Udp.IUdpSession, Letter.Udp.IUdpChannelFilter>;


namespace Letter.Udp
{
    sealed class UdpBootstrap : ADgramBootstrap<UdpOptions, IUdpSession, IUdpChannelFilter, IUdpChannel>, IUdpBootstrap
    {
        protected override Task<IUdpChannel> ChannelFactory(UdpOptions options, FilterGroupFactory groupFactory)
        {
            IUdpChannel channel = new UdpChannel(options, groupFactory);

            return Task.FromResult(channel);
        }
    }
}