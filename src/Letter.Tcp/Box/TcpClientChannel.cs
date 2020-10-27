using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


using FilterGroupFactory = Letter.Bootstrap.ChannelFilterGroupFactory<Letter.Tcp.Box.ITcpSession, Letter.Tcp.Box.ITcpChannelFilter>;

namespace Letter.Tcp.Box
{
    class TcpClientChannel : ATcpChannel, ITcpClientChannel
    {
        public TcpClientChannel(TcpClientOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature) 
            : base(groupFactory, sslFeature)
        {
            this.options = options;
        }
        
        private Socket connectSocket;
        private TcpClientOptions options;
        
        public EndPoint ConnectAddress { get; private set; }

        public async Task StartAsync(EndPoint address)
        {
            var ipEndPoint = address as IPEndPoint;
            if (ipEndPoint is null)
                throw new NotSupportedException("The SocketConnectionFactory only supports IPEndPoints for now.");

            if (connectSocket == null)
            {
                this.connectSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            
            await this.connectSocket.ConnectAsync(ipEndPoint);
        }
        
        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}