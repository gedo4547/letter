using System;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    sealed class KcpServerBootstrap : AKcpBootstrap<KcpServerOptions, IKcpServerChannel>, IKcpServerBootstrap
    {
        protected override async Task<IKcpServerChannel> ChannelFactoryAsync(KcpServerOptions options, Action<IFilterPipeline<IKcpSession>> handler)
        {
            var channel = (KcpChannel)await this.kcpBootstrap.CreateAsync();
            var serverChannel = new KcpServerChannel(options, channel, handler);

            return serverChannel;
        }
    }
}