using System.Net;

using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpSession : ISession
    {
        uint Conv { get; }
        
        EndPoint RemoteAddress { get; }
        
        void SendReliableAsync(object o);

        void SendUnreliableAsync(object o);
    }
}