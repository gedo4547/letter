using System.Buffers;
using System.Net;

namespace Letter
{
    public interface IDgramChannelFilterGroup<TSession, TFilter> : IChannelFilterGroup<TSession, TFilter>
        where TSession : ISession
        where TFilter : IDgramChannelFilter<TSession>
    {
        void OnChannelRead(TSession session, EndPoint remoteAddress, ref WrappedDgramReader reader);
        void OnChannelWrite(TSession session, EndPoint remoteAddress, ref WrappedDgramWriter writer, object obj);
        void OnChannelWrite(TSession session, EndPoint remoteAddress, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> buffer);
    }
}