using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpBootstrap<TOptions, TChannel> : IBootstrap<TOptions, IKcpSession, TChannel>
        where TOptions : KcpOptions, new()
        where TChannel : IKcpChannel<TOptions>
    {
    }
}