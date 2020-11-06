using System;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Udp
{
    sealed class UdpBootstrap : ABootstrap<UdpOptions, IUdpSession, IUdpChannel>, IUdpBootstrap
    {
        protected override Task<IUdpChannel> ChannelFactoryAsync(UdpOptions options, Action<IFilterPipeline<IUdpSession>> handler)
        {
            UdpChannel channel = new UdpChannel(options, handler);
            return Task.FromResult((IUdpChannel) channel);
        }
    }
}