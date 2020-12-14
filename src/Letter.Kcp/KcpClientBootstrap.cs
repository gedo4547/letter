using System;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    sealed class KcpClientBootstrap : AKcpBootstrap<KcpClientOptions, IKcpClientChannel>, IKcpClientBootstrap
    {
        protected override async Task<IKcpClientChannel> ChannelFactoryAsync(KcpClientOptions options, Action<IFilterPipeline<IKcpSession>> handler)
        {
            var udpChannel = await this.udpBootstrap.CreateAsync();
            IKcpClientChannel channel = new KcpClientChannel(options, udpChannel, handler);
            
            return channel;
        }
    }
}