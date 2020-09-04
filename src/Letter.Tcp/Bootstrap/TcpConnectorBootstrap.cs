using System;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public sealed class TcpConnectorBootstrap : ATcpBootstrap<TcpClientOptions>, ITcpConnectorBootstrap
    {
        public TcpConnectorBootstrap()
        {
             this.connector = TcpNetworkFactory.Connector();
             this.connector.ConfigureOptions((options) =>
             {
                 if (this.optionsFactory == null)
                 {
                     throw new NullReferenceException(nameof(this.optionsFactory));
                 }
                 
                 this.optionsFactory(options);
             });
        }

        private ITcpConnector connector;

        public override async Task StartAsync(EndPoint point)
        {
            this.connector.Build();
            var session = await this.connector.ConnectAsync(point);
            base.OnConnect(session);
        }
    }
}