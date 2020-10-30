using System;
using System.Collections.Generic;
using System.IO.Pipelines;

namespace Letter
{
    public interface IFilter<TSession> where TSession : ISession
    {
        void OnTransportActive(TSession session);
        
        void OnTransportInactive(TSession session);
        
        void OnTransportException(TSession session, Exception ex);
        
        void OnTransportRead(TSession session, ref WrappedReader reader, List<Object> args);
        
        void OnTransportWrite(TSession session, ref WrappedWriter writer, List<Object> args);
    }
}