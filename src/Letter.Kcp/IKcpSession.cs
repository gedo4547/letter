using System.Net;

using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpSession : ISession
    {
        uint Conv { get; }
        
        EndPoint RemoteAddress { get; }
        
        void SendSafeAsync(object o);

        void SendUnsafeAsync(object o);
    }
}