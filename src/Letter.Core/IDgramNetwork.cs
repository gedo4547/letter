namespace Letter
{
    public interface IDgramNetwork<TOptions, TChannel, TContext> : INetwork<TOptions, ChannelGroupFactoryDgramImpl<TContext, TChannel>, ChannelGroupDgramImpl<TContext, TChannel>, TChannel, TContext>
        where TChannel : IDgramChannel<TContext>
        where TOptions : IOptions
        where TContext : IContext
    {
        
    }
}