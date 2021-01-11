using System.Net;

using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpSession : ISession
    {
        uint CurrentConv { get; }
        
        EndPoint RemoteAddress { get; }
        
        void Send(object o);
    }
}