using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpServerChannel : ITcpServerChannel
    {
        public EndPoint BindAddress { get; }

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