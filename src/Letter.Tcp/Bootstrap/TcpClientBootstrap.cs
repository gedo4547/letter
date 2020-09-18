using System;
using System.Threading.Tasks;

using FilterGroupFactory = Letter.StreamChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    class TcpClientBootstrap : ATcpBootstrap<TcpClientOptions, ITcpClientChannel>, ITcpClientBootstrap
    {
        protected override Task<ITcpClientChannel> ChannelFactory(TcpClientOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature)
        {
            ITcpClientChannel channel = new TcpClientChannel(
                options,
                groupFactory, 
                sslFeature);

            return Task.FromResult(channel);
        }
    }
}