using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp.Box
{
    public class TcpSession : ITcpInternalSession
    {
        public string Id { get; }
        public BinaryOrder Order { get; }
        public EndPoint LoaclAddress { get; }
        public EndPoint RemoteAddress { get; }
        public MemoryPool<byte> MemoryPool { get; }
        public PipeScheduler Scheduler { get; }
        
        public Task StartAsync()
        {
            throw new System.NotImplementedException();
        }
        
        public Task WriteAsync(object o)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}