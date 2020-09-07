using System;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpClientBootstrap : ATcpBootstrap<TcpClientOptions>, ITcpClientBootstrap
    {
        public TcpClientBootstrap()
        {
            this.client = TcpNetworkFactory.Client();
            this.client.ConfigureOptions((options) =>
            {
                this.optionsFactory(options);
            });
        }

        private ITcpClient client;


        public override async Task StartAsync(EndPoint point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }
            
            this.client.Build();
            await this.client.ConnectAsync(point);

            base.OnConnect(this.client);
        }
    }
}