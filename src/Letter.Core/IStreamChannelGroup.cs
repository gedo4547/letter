using System.Buffers;

namespace Letter
{
    public interface IStreamChannelGroup<TSession, TChannel> : IChannelGroup<TSession, TChannel>
        where TSession : ISession
        where TChannel : IStreamChannel<TSession>
    {
        void OnChannelRead(TSession session, ref WrappedStreamReader reader);
        void OnChannelWrite(TSession session, ref WrappedStreamWriter writer, object obj);
        void OnChannelWrite(TSession session, ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> buffer);
    }
}