using System.Net;

namespace Letter
{
    public interface IStreamChannelFilter<TSession> : IChannelFilter<TSession>
        where TSession : ISession
    {
        void OnChannelRead(TSession session, ref WrappedStreamReader reader, ref ChannelArgs args);
        void OnChannelWrite(TSession session, ref WrappedStreamWriter writer, ref ChannelArgs args);
    }
}