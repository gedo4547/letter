using System.Buffers;
using System.Net;

namespace Letter
{
    public interface IStreamChannelFilterGroup<TSession, TFilter> : IChannelFilterGroup<TSession, TFilter>
        where TSession : ISession
        where TFilter : IStreamChannelFilter<TSession>
    {
        void OnFilterRead(TSession session, EndPoint remoteAddress, ref WrappedStreamReader reader);
        void OnFilterWrite(TSession session, EndPoint remoteAddress, ref WrappedStreamWriter writer, object obj);
        void OnFilterWrite(TSession session, EndPoint remoteAddress, ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> buffer);
    }
}