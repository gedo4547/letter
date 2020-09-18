using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    abstract class ATcpSession : ITcpSession
    {
        public string Id => throw new System.NotImplementedException();

        public EndPoint LoaclAddress => throw new System.NotImplementedException();

        public EndPoint RemoteAddress => throw new System.NotImplementedException();

        public MemoryPool<byte> MemoryPool => throw new System.NotImplementedException();

        public PipeScheduler Scheduler => throw new System.NotImplementedException();

        public abstract Task StartAsync();

        public Task WriteAsync(object obj)
        {
            throw new System.NotImplementedException();
        }

        public Task WriteAsync(ref ReadOnlySequence<byte> sequence)
        {
            throw new System.NotImplementedException();
        }

        public virtual ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}