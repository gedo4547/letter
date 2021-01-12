namespace Letter.Kcp
{
    public interface IKcpClosable
    {
        void OnSessionClosed(IKcpSession session);
    }
}
