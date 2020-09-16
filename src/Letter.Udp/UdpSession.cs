using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public class UdpSession : IUdpSession
    {
        public string Id { get; }
        public EndPoint LoaclAddress { get; }
        public EndPoint RemoteAddress { get; }
        public MemoryPool<byte> MemoryPool { get; }
        
        public Task CloseAsync()
        {
            
            throw new System.NotImplementedException();
        }
    }
}