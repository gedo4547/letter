using System;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpServerBootstrap : ATcpBootstrap<TcpServerBootstrapOptions>, ITcpServerBootstrap
    {
        public TcpServerBootstrap()
        {
             this.server = TcpFactory.Server();
             this.server.ConfigureOptions((options) =>
             {
                 TcpServerBootstrapOptions bootstrapOptions = new TcpServerBootstrapOptions();
                 this.optionsFactory(bootstrapOptions);
                 options = bootstrapOptions;
                 this.order = bootstrapOptions.Order;
             });
        }

        private ITcpServer server;
        
        public override Task StartAsync(EndPoint point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            this.server.Build();
            this.server.Bind(point);
            this.StartAcceptAsync().ConfigureAwait(false);
            
            return Task.CompletedTask;
        }


        protected async Task StartAcceptAsync()
        {
            while (true)
            {
                ITcpClient client = await this.server.AcceptAsync();
                if (client == null)
                {
                    break;
                }
                
                base.OnConnect(client);
            }
        }
    }
}