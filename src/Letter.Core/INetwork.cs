using System;

namespace Letter
{
    public interface INetwork<TOptions, TChannelGroupFactory, TChannelGroup, TChannel, TContext> : IAsyncDisposable
        where TOptions : IOptions
        where TContext : IContext
        where TChannel : IChannel<TContext>
        where TChannelGroup : AChannelGroup<TChannel, TContext>
        where TChannelGroupFactory : AChannelGroupFactory<TChannelGroup, TChannel, TContext>
    {
        void Initialize(TOptions options, TChannelGroupFactory factory);
    }
}