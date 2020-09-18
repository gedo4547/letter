using System;
using System.Net;
using System.Threading.Tasks;

using FilterGroupFactory = Letter.StreamChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    public class TcpServerChannel : ITcpServerChannel
    {
        public TcpServerChannel(TcpServerOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature)
        {
            this.options = options;
            this.sslFeature = sslFeature;
            this.groupFactory = groupFactory;

            if(sslFeature == null)
                this.sessionCreator = this.CreateTcpSession;
            else
                this.sessionCreator = this.CreateSslTcpSession;
        }

        public EndPoint BindAddress
        {
            get { return this.server.BindAddress; }
        }

        private SslFeature sslFeature;
        private TcpServerOptions options;
        private FilterGroupFactory groupFactory;
        private Func<ITcpClient, ATcpSession> sessionCreator;

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

        private ATcpSession CreateTcpSession(ITcpClient client)
        {
            var filterGroup = this.groupFactory.CreateFilterGroup();
            TcpSession session = new TcpSession(client, filterGroup);

            return session;
        }

        private ATcpSession CreateSslTcpSession(ITcpClient client)
        {
            var filterGroup = this.groupFactory.CreateFilterGroup();
            TcpSslSession session = new TcpSslSession(client, this.sslFeature, filterGroup);

            return session;
        }
        
        public async ValueTask DisposeAsync()
        {
            await this.server.DisposeAsync();

            await this.acceptTask;
        }
    }
}