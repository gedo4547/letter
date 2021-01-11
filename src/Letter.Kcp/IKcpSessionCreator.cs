using System.Net;

namespace Letter.Kcp
{
    public interface IKcpSessionCreator
    {
        bool IsActivate { get; }
        IKcpSession Create(uint conv, EndPoint remoteAddress, IKcpClosable closable);
    }
}
