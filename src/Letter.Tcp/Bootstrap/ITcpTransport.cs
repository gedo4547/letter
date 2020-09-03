using Letter.IO;

namespace Letter.Tcp
{
    public interface ITcpTransport : ITransport<ITcpSession, ITcpChannel, ITcpContext, WrappedStreamReader, WrappedStreamWriter>
    {
        
    }
}