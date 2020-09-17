

using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpServer : ITcpNetwork<TcpServerOptions>
    {
        EndPoint BindAddress { get; }

        void Bind(EndPoint point);

        ValueTask<ITcpClient> AcceptAsync(CancellationToken cancellationToken = default);
    }
}