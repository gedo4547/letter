using System;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    sealed class KcpServerBootstrap : AKcpBootstrap<KcpServerOptions, IKcpServerChannel>, IKcpServerBootstrap
    {
        protected override Task<IKcpServerChannel> ChannelFactoryAsync(KcpServerOptions options, Action<IFilterPipeline<IKcpSession>> handler)
        {
            throw new NotImplementedException();
        }
    }
}