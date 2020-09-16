using System;

namespace Letter
{
    public interface IDgramBootstrap<TOptions, TSession, TFilter, TChannel> : IBootstrap<TOptions, TChannel>
        where TOptions : class, IOptions, new()
        where TSession : ISession
        where TFilter : IDgramChannelFilter<TSession>
        where TChannel : IChannel
    {
        void AddChannelFilter<TChannelFilter>() where TChannelFilter : TFilter, new();

        void AddChannelFilter(Func<TFilter> filterFactory);
    }
}