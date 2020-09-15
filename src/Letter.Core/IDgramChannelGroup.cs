using System.Buffers;

namespace Letter.Box.ssss
{
    public interface IDgramChannelGroup<TSession, TChannel> : IChannelGroup<TSession, TChannel>
        where TSession : ISession
        where TChannel : IDgramChannel<TSession>
    {
        void OnChannelRead(TSession session, ref WrappedDgramReader reader);
        void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, object obj);
        void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> buffer);
    }
}