using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpBootstrap<TOptions, TChannel> : IBootstrap<TOptions, IKcpSession, TChannel>
        where TOptions : AKcpOptions, new()
        where TChannel : IKcpChannel
    {
    }
}