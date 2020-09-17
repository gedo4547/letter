using System.Buffers;

namespace Letter
{
    public interface IDgramChannelFilterGroup<TSession, TFilter> : IChannelFilterGroup<TSession, TFilter>
        where TSession : ISession
        where TFilter : IDgramChannelFilter<TSession>
    {
        void OnChannelRead(TSession session, ref WrappedDgramReader reader);
        void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, object obj);
        void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> buffer);
    }
}