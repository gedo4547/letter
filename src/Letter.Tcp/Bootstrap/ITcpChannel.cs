using Letter.IO;

namespace Letter.Tcp
{
    public interface ITcpChannel : IChannel<ITcpContext, WrappedStreamReader, WrappedStreamWriter>
    {
        
    }
}