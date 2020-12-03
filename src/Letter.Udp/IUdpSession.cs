using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Udp
{
    public interface IUdpSession : ISession
    {
        EndPoint RcvAddress { get; }
        EndPoint SndAddress { get; }
        
        void Write(EndPoint remoteAddress, object obj);
        
        ValueTask FlushAsync();
    }
}