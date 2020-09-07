using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpClient : IClient<TcpClientOptions>
    {
        IDuplexPipe Transport { get; }
        
        MemoryPool<byte> MemoryPool { get; }
        
        ValueTask ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default);
    }
}