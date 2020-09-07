using System;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpServerBootstrap : ATcpBootstrap<TcpServerOptions>, ITcpServerBootstrap
    {
        public TcpServerBootstrap()
        {
             this.server = TcpNetworkFactory.Server();
             this.server.ConfigureOptions((options) =>
             {
                 this.optionsFactory(options);
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