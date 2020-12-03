using System;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    sealed class KcpClientBootstrap : AKcpBootstrap<KcpClientOptions, IKcpClientChannel>, IKcpClientBootstrap
    {
        protected override Task<IKcpClientChannel> ChannelFactoryAsync(KcpClientOptions options, Action<IFilterPipeline<IKcpSession>> handler)
        {
            throw new NotImplementedException();
        }
    }
}