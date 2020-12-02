using Letter.IO;

namespace Letter.Kcp
{
    abstract class AKcpBootstrap<TOptions> : ABootstrap<TOptions, IKcpSession, IKcpChannel>, IKcpBootstrap<TOptions>
        where TOptions : AKcpOptions, new()
    {
    }
}