using System.Net;

namespace Letter
{
    public interface IDgramChannelFilter<TSession> : IChannelFilter<TSession>
        where TSession : ISession
    {
        void OnChannelRead(TSession session, EndPoint remoteAddress, ref WrappedDgramReader reader, ref ChannelArgs args);
        
        void OnChannelWrite(TSession session, EndPoint remoteAddress, ref WrappedDgramWriter writer, ref ChannelArgs args);
    }
}