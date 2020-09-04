using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Letter.Box;

namespace Letter.Tcp.Box
{
    public interface ITcpClient : IClient<TcpConnectorOptions>
    {
        IDuplexPipe Transport { get; }
        MemoryPool<byte> MemoryPool { get; }
        
        ValueTask ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default);
    }
}