using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpListener : ITcpNetwork<TcpServerOptions>
    {
        void Bind(EndPoint point);
        
        ValueTask<ITcpSession> AcceptAsync(CancellationToken cancellationToken = default);

        ValueTask UnbindAsync(CancellationToken cancellationToken = default);
    }
}