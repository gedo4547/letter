using System;

namespace Letter
{
    public interface IChannelGroup<TSession, TChannel> : IAsyncDisposable
        where TSession : ISession
        where TChannel : IChannel<TSession>
    {
        void OnChannelActive(TSession session);
        void OnChannelInactive(TSession session);
        void OnChannelException(TSession session, Exception ex);
    }
}