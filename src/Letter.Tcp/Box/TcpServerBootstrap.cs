using System.Threading.Tasks;


using FilterGroupFactory = Letter.Bootstrap.ChannelFilterGroupFactory<Letter.Tcp.Box.ITcpSession, Letter.Tcp.Box.ITcpChannelFilter>;

namespace Letter.Tcp.Box
{
    sealed class TcpServerBootstrap : ATcpBootstrap<TcpServerOptions, ITcpServerChannel>, ITcpServerBootstrap
    {
        protected override Task<ITcpServerChannel> ChannelFactory(TcpServerOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature)
        {
            throw new System.NotImplementedException();
        }
    }
}