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
        private Func<ITcpClient, IInternalTcpSession> sessionCreator;

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

        private IInternalTcpSession CreateTcpSession(ITcpClient client)
        {
            var filterGroup = this.groupFactory.CreateFilterGroup();
            TcpSession session = new TcpSession(client, client.Transport, filterGroup);

            return session;
        }

        private IInternalTcpSession CreateSslTcpSession(ITcpClient client)
        {
            var inputPipeOptions = StreamPipeOptionsHelper.ReaderOptionsCreator(client.MemoryPool);
            var outputPipeOptions = StreamPipeOptionsHelper.WriterOptionsCreator(client.MemoryPool);
            var sslDuplexPipe = new SslStreamDuplexPipe(
                client.Transport, 
                inputPipeOptions, 
                outputPipeOptions, 
                this.sslFeature.sslStreamFactory);
            
            var filterGroup = this.groupFactory.CreateFilterGroup();
            TcpSslSession session = new TcpSslSession(client, sslDuplexPipe, this.sslFeature, filterGroup);

            return session;
        }
        
        public async ValueTask DisposeAsync()
        {
            await this.server.DisposeAsync();

            await this.acceptTask;
        }
    }
}