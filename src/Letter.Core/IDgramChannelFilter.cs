namespace Letter
{
    public interface IDgramChannelFilter<TSession> : IChannelFilter<TSession>
        where TSession : ISession
    {
        void OnChannelRead(TSession session, ref WrappedDgramReader reader, ref FilterArgs args);
        
        void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, ref FilterArgs args);
    }
}