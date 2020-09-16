using System.Net;

namespace Letter
{
    public interface IDgramChannelFilter<TSession> : IChannelFilter<TSession>
        where TSession : ISession
    {
        void OnChannelRead(TSession session, ref WrappedDgramReader reader, ref ChannelArgs args);
        
        void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, ref ChannelArgs args);
    }
}