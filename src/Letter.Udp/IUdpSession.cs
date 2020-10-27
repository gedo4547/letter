using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp.Box
{
    public interface IUdpSession : Letter.Bootstrap.ISession
    {
        EndPoint RcvAddress { get; }
        EndPoint SndAddress { get; }
        
        
        Task WriteAsync(EndPoint remoteAddress, object obj);
    }
}