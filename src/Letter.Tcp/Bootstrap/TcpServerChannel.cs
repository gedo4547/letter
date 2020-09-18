using System;
using System.Net;
using System.Threading.Tasks;

using FilterGroupFactory = Letter.StreamChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    class TcpServerChannel : ATcpChannel, ITcpServerChannel
    {
        public TcpServerChannel(TcpServerOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature) 
            : base(groupFactory, sslFeature)
        {
            this.options = options;
        }

        public EndPoint BindAddress
        {
            get { return this.server.BindAddress; }
        }
        
        private TcpServerOptions options;
        
        private ITcpServer server;
        private Task acceptTask;

        public Task StartAsync(EndPoint address)
        {
            this.server = TcpFactory.Server();
            this.server.ConfigureOptions(options =>
            {
                options = this.options;
            });
            this.server.Build();
            this.server.Bind(address);
            
            this.acceptTask = this.AcceptAsync();
            
            return Task.CompletedTask;
        }
        
        private async Task AcceptAsync()
        {
            while (true)
            {
                var client = await this.server.AcceptAsync();
                if (client == null)
                {
                    break;
                }
                
                var session = this.sessionCreator(client);
                await session.StartAsync();
            }
        }
        
        public async ValueTask DisposeAsync()
        {
            await this.server.DisposeAsync();

            await this.acceptTask;
            
            base.Dispose();
        }
    }
}