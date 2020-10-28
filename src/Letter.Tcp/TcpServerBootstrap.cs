using System.Threading.Tasks;


using FilterGroupFactory = Letter.ChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    sealed class TcpServerBootstrap : ATcpBootstrap<TcpServerOptions, ITcpServerChannel>, ITcpServerBootstrap
    {
        protected override Task<ITcpServerChannel> ChannelFactory(TcpServerOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature)
        {
            return Task.FromResult((ITcpServerChannel) new TcpServerChannel(options, groupFactory, sslFeature));
        }
    }
}