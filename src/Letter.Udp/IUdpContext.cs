using System.Buffers;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public interface IUdpContext : IContext
    {
        Task WriteAsync(EndPoint remote, object o);
        
        Task WriteAsync(EndPoint remote, ref ReadOnlySequence<byte> sequence);
    }
}