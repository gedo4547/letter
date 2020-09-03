using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public sealed class TcpConnectorBootstrap : ATcpBootstrap<TcpConnectorOptions>, ITcpConnectorBootstrap
    {
        public TcpConnectorBootstrap()
        {
             this.connector = TcpNetworkFactory.Connector();
        }

        private ITcpConnector connector;

        public override Task StartAsync(EndPoint point)
        {
            this.connector.Build();
            
            
            return Task.CompletedTask;
        }
    }
}