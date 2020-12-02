using System;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    sealed class KcpClientBootstrap : AKcpBootstrap<KcpClientOptions>, IKcpClientBootstrap
    {
        protected override Task<IKcpChannel> ChannelFactoryAsync(KcpClientOptions options, Action<IFilterPipeline<IKcpSession>> handler)
        {
            throw new NotImplementedException();
        }
    }
}