using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpClientChannel : ITcpClientChannel
    {
        public EndPoint ConnectAddress { get; }

        public Task StartAsync(EndPoint address)
        {
            throw new System.NotImplementedException();
        }
        
        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}