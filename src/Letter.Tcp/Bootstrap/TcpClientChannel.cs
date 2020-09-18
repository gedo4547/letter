using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpClientChannel : ATcpChannel, ITcpClientChannel
    {
        public TcpClientChannel(StreamChannelFilterGroupFactory<ITcpSession, ITcpChannelFilter> groupFactory, SslFeature sslFeature)
            : base(groupFactory, sslFeature)
        {
        }
        
        public EndPoint ConnectAddress { get; }

        public Task StartAsync(EndPoint address)
        {
            throw new System.NotImplementedException();
        }
        
        public ValueTask DisposeAsync()
        {
            
            base.Dispose();

            return default;

        }
    }
}