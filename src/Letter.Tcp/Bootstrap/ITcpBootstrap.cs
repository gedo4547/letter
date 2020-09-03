using Letter.IO;

namespace Letter.Tcp
{
    public interface ITcpBootstrap<TOptions> : IBootstrap<ITcpTransport, ITcpContext, ITcpSession, TOptions,ITcpChannel, WrappedStreamReader, WrappedStreamWriter>
        where TOptions : ATcpOptions
    {
        
    }
}