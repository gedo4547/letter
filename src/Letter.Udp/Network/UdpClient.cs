using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public class UdpClient : ANetwork<UdpClientOptions>, IUdpClient
    {
        public UdpClient() : base(new UdpClientOptions())
        {
        }
        
        public string Id { get; }
        public EndPoint LocalAddress { get; }
        public EndPoint RemoteAddress { get; }
        
        private Socket socket;

        public void AddExceptionListener(Action<Exception> onException)
        {
            throw new NotImplementedException();
        }

        public void Bind(EndPoint point)
        {
            this.socket = new Socket(point.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            this.socket.Bind(point);
        }
        
        public override ValueTask CloseAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }
    }
}