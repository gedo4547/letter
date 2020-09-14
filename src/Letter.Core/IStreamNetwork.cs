using Letter.IO;

namespace Letter
{
    public interface IStreamNetwork<TOptions, TContext, TChannel> : INetwork<TOptions, TChannel, TContext>
        where TOptions : IOptions
        where TContext : IContext
        where TChannel : IStreamChannel<TContext>
    {
        
    }
}