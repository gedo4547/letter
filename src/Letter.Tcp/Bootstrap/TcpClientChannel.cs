using System.Net;
using System.Threading.Tasks;

using FilterGroupFactory = Letter.StreamChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    class TcpClientChannel : ATcpChannel, ITcpClientChannel
    {
        public TcpClientChannel(TcpClientOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature)
            : base(groupFactory, sslFeature)
        {
        }
        
        public EndPoint ConnectAddress { get; }

        private ITcpClient client;

        public async Task StartAsync(EndPoint address)
        {
            this.client = TcpFactory.Client();

            this.client.ConfigureOptions(options=>{

            });
            this.client.Build();
            await this.client.ConnectAsync(address);
            var session = this.sessionCreator(this.client); 
            await session.StartAsync();
        }
        
        public ValueTask DisposeAsync()
        {
            
            base.Dispose();

            return default;

        }
    }
}