using System.Threading.Tasks;

using FilterGroupFactory = Letter.Bootstrap.ChannelFilterGroupFactory<Letter.Tcp.Box.ITcpSession, Letter.Tcp.Box.ITcpChannelFilter>;

namespace Letter.Tcp.Box
{
    class TcpClientBootstrap : ATcpBootstrap<TcpClientOptions, ITcpClientChannel>, ITcpClientBootstrap
    {
        protected override Task<ITcpClientChannel> ChannelFactory(TcpClientOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature)
        {
            return Task.FromResult((ITcpClientChannel) new TcpClientChannel(options, groupFactory, sslFeature));
        }
    }
}