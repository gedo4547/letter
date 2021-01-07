using System.Net;

using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpSession : ISession
    {
        uint CurrentConv { get; set; }
        
        EndPoint RemoteAddress { get; }
        
        void Send(object o);
    }
}