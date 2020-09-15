namespace Letter
{
    public interface IStreamNetwork<TOptions, TChannel, TContext> : INetwork<TOptions, ChannelGroupFactoryStreamImpl<TContext, TChannel>, ChannelGroupStreamImpl<TContext, TChannel>, TChannel, TContext>
        where TOptions : IOptions
        where TContext : IContext
        where TChannel : IStreamChannel<TContext>
    {
        
    }
}