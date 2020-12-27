using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpSession : ISession
    {
        uint Conv { get; }
        
        EndPoint RemoteAddress { get; }
        
        void Write(object o);

        ValueTask FlushAsync();
    }
}