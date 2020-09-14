using Letter.IO;

namespace Letter
{
    public interface IDgramNetwork<TOptions, TContext> : INetwork<TOptions, IDgramChannel<TContext>, TContext>
        where TOptions: IOptions
        where TContext : class, IContext
    {
        
    }
}