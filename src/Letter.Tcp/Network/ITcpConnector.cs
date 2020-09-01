using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpConnector : ITcpNetwork<TcpConnectorOptions>
    {
        ValueTask<ITcpSession> ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default);
    }
}