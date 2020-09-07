using System.Buffers;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public partial class TcpContext : ITcpContext
    {
        public string Id
        {
            get { return this.client.Id; }
        }
        
        public EndPoint LoaclAddress 
        {
            get { return this.client.LocalAddress; }
        }
        
        public EndPoint RemoteAddress 
        {
            get { return this.client.RemoteAddress; }
        }
        
        public MemoryPool<byte> MemoryPool 
        {
            get { return this.client.MemoryPool; }
        }

        private ITcpClient client;
        
        public void Initialize(ITcpClient client)
        {
            this.client = client;
        }

        public Task WriteAsync(object o)
        {
            throw new System.NotImplementedException();
        }

        public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }
        
        public Task CloseAsync()
        {
            throw new System.NotImplementedException();
        }
        
        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}