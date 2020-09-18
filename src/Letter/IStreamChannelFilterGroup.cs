﻿using System.Buffers;

namespace Letter
{
    public interface IStreamChannelFilterGroup<TSession, TFilter> : IChannelFilterGroup<TSession, TFilter>
        where TSession : ISession
        where TFilter : IStreamChannelFilter<TSession>
    {
        void OnChannelRead(TSession session, ref WrappedStreamReader reader);
        void OnChannelWrite(TSession session, ref WrappedStreamWriter writer, object obj);
        void OnChannelWrite(TSession session, ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> buffer);
    }
}