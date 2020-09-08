using System;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpClientBootstrap : ATcpBootstrap<TcpClientBootstrapOptions>, ITcpClientBootstrap
    {
        public TcpClientBootstrap()
        {
            this.client = TcpFactory.Client();
            this.client.ConfigureOptions((options) =>
            {
                TcpClientBootstrapOptions bootstrapOptions = new TcpClientBootstrapOptions();
                this.optionsFactory(bootstrapOptions);
                options = bootstrapOptions;
                this.order = bootstrapOptions.Order;
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