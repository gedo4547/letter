using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpListener : ITcpNetwork<TcpListenerOptions>
    {
        void Bind(EndPoint point);
        
        ValueTask<ITcpSession> AcceptAsync(CancellationToken cancellationToken = default);
        
        
    }
}