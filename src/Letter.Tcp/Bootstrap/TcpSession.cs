using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpSession : ITcpSession
    {
        public string Id { get; }
        public EndPoint LoaclAddress { get; }
        public EndPoint RemoteAddress { get; }
        public MemoryPool<byte> MemoryPool { get; }
        public PipeScheduler Scheduler { get; }
        
        public Task WriteAsync(object obj)
        {
            throw new System.NotImplementedException();
        }

        public Task WriteAsync(ref ReadOnlySequence<byte> sequence)
        {
            throw new System.NotImplementedException();
        }
        
        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}