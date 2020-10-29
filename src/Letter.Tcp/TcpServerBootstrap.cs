using System;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    sealed class TcpServerBootstrap : ATcpBootstrap<TcpServerOptions, ITcpServerChannel>, ITcpServerBootstrap
    {
        protected override Task<ITcpServerChannel> ChannelFactoryAsync(TcpServerOptions options, Action<IFilterPipeline<ITcpSession>> handler, SslFeature sslFeature)
        {
            return Task.FromResult((ITcpServerChannel) new TcpServerChannel(options, handler, sslFeature));
        }
    }
}