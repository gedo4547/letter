namespace Letter
{
    public interface IDgramNetwork<TOptions, TContext, TChannel> : INetwork<TOptions, TChannel , TContext>
        where TOptions: IOptions
        where TContext : IContext
        where TChannel : IDgramChannel<TContext>
    {
        
    }
}