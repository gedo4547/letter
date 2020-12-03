using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    abstract class AKcpBootstrap<TOptions, TChannel> : ABootstrap<TOptions, IKcpSession, TChannel>, IKcpBootstrap<TOptions, TChannel>
        where TOptions : AKcpOptions, new()
        where TChannel : IKcpChannel
    {
        public AKcpBootstrap()
        {
            this.udpBootstrap = UdpFactory.Bootstrap();
        }

        private IUdpBootstrap udpBootstrap;
    }
}