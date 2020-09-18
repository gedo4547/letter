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
        }

        public EndPoint BindAddress
        {
            get { return this.server.BindAddress; }
        }

        private SslFeature sslFeature;
        private TcpServerOptions options;
        private FilterGroupFactory groupFactory;

        private ITcpServer server;

        public Task StartAsync(EndPoint address)
        {
            this.server = TcpFactory.Server();
            this.server.Bind(address);
            
            this.AcceptAsync().NoAwait();
            
            return Task.CompletedTask;
        }
        
        private async Task AcceptAsync()
        {
            while (true)
            {
                var client = await this.server.AcceptAsync();
                
            }
        }



        public async ValueTask DisposeAsync()
        {
            await this.server.DisposeAsync();
        }
    }
}