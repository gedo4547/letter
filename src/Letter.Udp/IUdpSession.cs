using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public interface IUdpSession : ISession
    {
        EndPoint RcvAddress { get; }
        EndPoint SndAddress { get; }
        
        Task WriteAsync(EndPoint remoteAddress, object obj);
        
        Task FlushAsync();
    }
}