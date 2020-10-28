using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpSession : ATcpSession
    {
        public TcpSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool, FilterPipeline<ITcpSession> filterPipeline) 
            : base(socket, options, scheduler, pool, filterPipeline)
        {
        }
        
        public override Task StartAsync()
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteAsync(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}