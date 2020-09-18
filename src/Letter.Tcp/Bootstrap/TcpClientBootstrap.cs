using System;
using System.Threading.Tasks;

using FilterGroupFactory = Letter.StreamChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    public class TcpClientBootstrap : ATcpBootstrap<TcpClientOptions, ITcpClientChannel>, ITcpClientBootstrap
    {
        protected override Task<ITcpClientChannel> ChannelFactory(TcpClientOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature)
        {
            throw new NotImplementedException();
        }
    }
}