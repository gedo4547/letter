using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public interface IUdpContext : IContext
    {
        Task WriteAsync(EndPoint remote, object o);
        Task WriteAsync(EndPoint remote, byte[] buffer, int offset, int count);
    }
}