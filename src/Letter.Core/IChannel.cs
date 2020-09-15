using System;

namespace Letter
{
    public interface IChannel<TSession> where TSession : ISession
    {
        void OnChannelActive(TSession session);
        void OnChannelInactive(TSession session);
        void OnChannelException(TSession session, Exception ex);
    }
}