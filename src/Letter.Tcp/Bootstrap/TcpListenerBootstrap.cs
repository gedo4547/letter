using System;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpListenerBootstrap : ATcpBootstrap<TcpServerOptions>, ITcpListenerBootstrap
    {
        public TcpListenerBootstrap()
        {
            this.listener = TcpNetworkFactory.Listener();
            this.listener.ConfigureOptions((options) =>
            {
                if (this.optionsFactory == null)
                {
                    throw new NullReferenceException(nameof(this.optionsFactory));
                }
                 
                this.optionsFactory(options);
            });
        }

        private ITcpListener listener;
        
        public override Task StartAsync(EndPoint point)
        {
            this.listener.Build();
            this.listener.Bind(point);

            this.StartAcceptAsync().ConfigureAwait(false);
            
            return Task.CompletedTask;
        }

        private async Task StartAcceptAsync()
        {
            while (true)
            {
                var session = await this.listener.AcceptAsync();
                if (session == null)
                {
                    break;
                }
                
                base.OnConnect(session);
            }
        }

        public override async Task StopAsync()
        {
            await this.listener.StopAsync();
            await base.StopAsync();
        }
    }
}