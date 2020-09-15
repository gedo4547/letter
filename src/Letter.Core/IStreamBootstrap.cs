using Letter.IO;

namespace Letter
{
    public interface IStreamBootstrap<TOptions, TContext, TChannel> : IBootstrap<TOptions, TChannel, TContext>
        where TOptions : IOptions
        where TContext : IContext
        where TChannel : IStreamChannel<TContext>
    {
        
    }
}