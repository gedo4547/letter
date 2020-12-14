using System;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    sealed class KcpServerBootstrap : AKcpBootstrap<KcpServerOptions, IKcpServerChannel>, IKcpServerBootstrap
    {
        protected override async Task<IKcpServerChannel> ChannelFactoryAsync(KcpServerOptions options,
            Action<IFilterPipeline<IKcpSession>> handler)
        {
            var udpChannel = await this.udpBootstrap.CreateAsync();
            IKcpServerChannel channel = new KcpServerChannel(options, udpChannel, handler);

            return channel;
        }
    }
}