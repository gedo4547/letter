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
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            this.socket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            
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