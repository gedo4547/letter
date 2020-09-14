using Letter.IO;

namespace Letter
{
    public interface IStreamNetwork<TOptions, TContext> : INetwork<TOptions, IStreamChannel<TContext>, TContext>
        where TOptions : IOptions
        where TContext : class, IContext
    {
        
    }
}