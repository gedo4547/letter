﻿using System;
using Letter.Box;

namespace Letter.Bootstrap
{
    public interface IChannelFilter<TSession> where TSession : ISession
    {
        void OnChannelActive(TSession session);
        
        void OnChannelInactive(TSession session);
        
        void OnChannelException(TSession session, Exception ex);
        
        void OnChannelRead(TSession session, ref WrappedReader reader, object args);
        
        void OnChannelWrite(TSession session, ref WrappedWriter writer, object args);
    }
}