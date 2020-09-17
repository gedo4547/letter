using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpClientChannel : ITcpClientChannel
    {
        public Task ConnectAsync(EndPoint address)
        {
            throw new System.NotImplementedException();
        }
        
        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}