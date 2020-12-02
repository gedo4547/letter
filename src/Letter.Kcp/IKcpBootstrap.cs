using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpBootstrap<TOptions> : IBootstrap<TOptions, IKcpSession, IKcpChannel>
        where TOptions : AKcpOptions, new()
    {
    }
}