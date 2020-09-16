using Letter.IO;

namespace Letter.Tcp
{
    public interface ITcpFilter : IFilter<ITcpContext, WrappedStreamReader, WrappedStreamWriter>
    {
        
    }
}