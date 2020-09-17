using System;

namespace Letter
{
    public interface IChannelFilterGroup<TSession, TFilter> : IAsyncDisposable
        where TSession : ISession
        where TFilter : IChannelFilter<TSession>
    {
        void OnChannelActive(TSession session);
        void OnChannelInactive(TSession session);
        void OnChannelException(TSession session, Exception ex);
    }
}