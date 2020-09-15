namespace Letter
{
    public interface IDgramBootstrap<TOptions, TContext, TChannel> : IBootstrap<TOptions, TChannel , TContext>
        where TOptions : IOptions
        where TContext : IContext
        where TChannel : IDgramChannel<TContext>
    {
        
    }
}