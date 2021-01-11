namespace Letter.Kcp
{
    public interface IKcpClosable
    {
        void Close(IKcpSession session);
    }
}
