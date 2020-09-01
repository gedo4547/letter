using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpSession : ITcpSession
    {
        public TcpSession(Socket socket,
            MemoryPool<byte> memoryPool,
            PipeScheduler scheduler,
            ISocketsTrace trace,
            long? maxReadBufferSize = null,
            long? maxWriteBufferSize = null,
            bool waitForData = true)
        {
            
        }




        public string Id { get; }
        
        
        
        
        public Task CloseAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}