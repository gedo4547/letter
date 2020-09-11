using Letter.IO;

namespace Letter
{
    public interface IStreamNetwork<TOptions, TContext> : INetwork<TOptions, IStreamChannel<TContext>, TContext, WrappedStreamReader, WrappedStreamWriter>
        where TOptions : IOptions
        where TContext : class, IContext
    {
        
    }
}