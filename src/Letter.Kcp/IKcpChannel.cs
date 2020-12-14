using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpChannel<TOptions> : IChannel<IKcpSession, TOptions>
        where TOptions : IOptions
    {
        
    }
}