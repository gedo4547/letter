using Letter.IO;

namespace Letter
{
    public interface IDgramNetwork<TOptions, TContext> : INetwork<TOptions, IDgramChannel<TContext>, TContext, WrappedDgramReader, WrappedDgramWriter>
        where TOptions: IOptions
        where TContext : class, IContext
    {
        
    }
}