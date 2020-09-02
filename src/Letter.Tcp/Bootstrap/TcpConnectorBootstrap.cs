using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpConnectorBootstrap : ATcpBootstrap<TcpConnectorOptions>, ITcpConnectorBootstrap
    {
        public TcpConnectorBootstrap()
        {
            this.connector = TcpNetworkFactory.Connector();
            this.connector.ConfigureOptions(this.optionsFactory);
        }

        private ITcpConnector connector;

        public override async Task StartAsync(EndPoint point)
        {
            this.connector.ConfigureLogger(this.trace);
            this.connector.Build();
            ITcpSession session = await this.connector.ConnectAsync(point);
            base.Connected(session);
        }

        public override async Task StopAsync()
        {
            if (this.connector != null)
            {
                await this.connector.StopAsync();
                this.connector = null;
            }
            
            await base.StopAsync();
        }
    }
}