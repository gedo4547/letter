using System;

namespace Letter
{
    public interface IChannelFilterGroup<TSession, TFilter> : IAsyncDisposable
        where TSession : ISession
        where TFilter : IChannelFilter<TSession>
    {
        void OnFilterActive(TSession session);
        void OnFilterInactive(TSession session);
        void OnFilterException(TSession session, Exception ex);
    }
}