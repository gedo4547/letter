using Letter.IO;

namespace Letter
{
    public interface IDgramChannel<TContext> : IChannel<TContext, WrappedDgramReader, WrappedDgramWriter>
        where TContext : class, IContext
    {
        
    }
}