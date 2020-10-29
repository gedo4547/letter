using System;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpClientBootstrap : ATcpBootstrap<TcpClientOptions, ITcpClientChannel>, ITcpClientBootstrap
    {
        protected override Task<ITcpClientChannel> ChannelFactoryAsync(TcpClientOptions options, Action<IFilterPipeline<ITcpSession>> handler, SslFeature sslFeature)
        {
            return Task.FromResult((ITcpClientChannel) new TcpClientChannel(options, handler, sslFeature));
        }
    }
}