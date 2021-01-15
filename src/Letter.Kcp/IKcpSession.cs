using System.Net;

using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpSession : ISession
    {
        uint Conv { get; }
        
        EndPoint RemoteAddress { get; }
        
        void SafeSendAsync(object o);

        void UnsafeSendAsync(EndPoint remoteAddress, object o);
    }
}