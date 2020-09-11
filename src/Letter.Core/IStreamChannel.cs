using Letter.IO;

namespace Letter
{
    public interface IStreamChannel<TContext> : IChannel<TContext, WrappedStreamReader, WrappedStreamWriter>
        where TContext : class, IContext
    {
        
    }
}