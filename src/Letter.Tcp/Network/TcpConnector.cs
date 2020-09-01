using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpConnector : ATcpNetwork<TcpConnectorOptions>, ITcpConnector
    {
        public TcpConnector(TcpConnectorOptions options) : base(options)
        {
        }

        public override void Build()
        {
            base.Build();
        }

        public ValueTask<ITcpSession> ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public override Task StopAsync()
        {
            return base.StopAsync();
        }
    }
}