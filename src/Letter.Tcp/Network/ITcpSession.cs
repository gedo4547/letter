using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading;

namespace Letter.Tcp
{
    public interface ITcpSession : ISession
    {
        EndPoint LocalEndPoint { get; }
        EndPoint RemoteEndPoint { get; }
        
        MemoryPool<byte> MemoryPool { get; }
        
        IDuplexPipe Transport { get; }
        IDuplexPipe Application { get; }
        
        CancellationToken ConnectionClosed { get; }
    }
}