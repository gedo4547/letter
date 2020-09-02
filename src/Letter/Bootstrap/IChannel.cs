using System;

namespace Letter
{
    public interface IChannel<TSession> where TSession : ISession
    {
        void OnTransportActive(TSession session);
        void OnTransportInactive(TSession session);
        void OnTransportException(TSession session, Exception ex);
    }
}