namespace Letter.Box.ssss
{
    public interface IStreamChannel<TSession> : IChannel<TSession>
        where TSession : ISession
    {
        void OnChannelRead(TSession session, ref WrappedStreamReader reader, ref ChannelArgs args);
        void OnChannelWrite(TSession session, ref WrappedStreamWriter writer, ref ChannelArgs args);
    }
}