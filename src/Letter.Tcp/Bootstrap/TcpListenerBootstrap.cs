using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpListenerBootstrap : ATcpBootstrap<TcpListenerOptions>, ITcpListenerBootstrap
    {
        public TcpListenerBootstrap()
        {
            this.listener = TcpNetworkFactory.Listener();
            this.listener.ConfigureOptions(this.optionsFactory);
        }
        
        private ITcpListener listener;
        
        public override Task StartAsync(EndPoint point)
        {
            this.listener.ConfigureLogger(this.trace);
            this.listener.Build();
            this.listener.Bind(point);

            this.StartAcceptAsync();
            
            return Task.CompletedTask;
        }

        private async void StartAcceptAsync()
        {
            while (true)
            {
                ITcpSession session = await this.listener.AcceptAsync();
                if (session == null) break;
                
                base.Connected(session);
            }
        }

        public override async Task StopAsync()
        {
            if (this.listener != null)
            {
                await this.listener.StopAsync();
                this.listener = null;
            }
            
            await base.StopAsync();
        }
    }
}