using System.Threading.Tasks;

using FilterGroupFactory = Letter.ChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    class TcpClientBootstrap : ATcpBootstrap<TcpClientOptions, ITcpClientChannel>, ITcpClientBootstrap
    {
        protected override Task<ITcpClientChannel> ChannelFactory(TcpClientOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature)
        {
            return Task.FromResult((ITcpClientChannel) new TcpClientChannel(options, groupFactory, sslFeature));
        }
    }
}