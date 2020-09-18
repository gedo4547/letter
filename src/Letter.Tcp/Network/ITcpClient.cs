using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpClient : ITcpNetwork<TcpClientOptions>
    {
        string Id { get; }
        IDuplexPipe Transport { get; }
        MemoryPool<byte> MemoryPool { get; }
        EndPoint LocalAddress { get; }
        EndPoint RemoteAddress { get; }
        PipeScheduler Scheduler{ get; }

        void AddClosedListener(Action<ITcpClient> onClosed);
        void AddExceptionListener(Action<Exception> onException);
        ValueTask ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default);
    }
}