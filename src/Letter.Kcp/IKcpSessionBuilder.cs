using System.Net;

namespace Letter.Kcp
{
    interface IKcpSessionBuilder
    {
        bool IsActivate { get; }

        IKcpSession Build(uint conv, EndPoint remoteAddress, IKcpClosable closable);
    }
}
