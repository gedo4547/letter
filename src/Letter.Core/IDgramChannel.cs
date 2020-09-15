namespace Letter.Box.ssss
{
    public interface IDgramChannel<TSession> : IChannel<TSession>
        where TSession : ISession
    {
        void OnChannelRead(TSession session, ref WrappedDgramReader reader, ref ChannelArgs args);
        
        void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, ref ChannelArgs args);
    }
}