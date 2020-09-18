using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

using FilterGroup = Letter.StreamChannelFilterGroup<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    class TcpSession : ATcpSession
    {
        public TcpSession(ITcpClient client, FilterGroup filterGroup)
        {
            this.client = client;
            this.filterGroup = filterGroup;

            this.client.AddClosedListener(this.OnClientClosed);
            this.client.AddExceptionListener(this.OnClientException);
        }

        private ITcpClient client;
        private FilterGroup filterGroup;

        public override Task StartAsync()
        {
            // this.client.Transport

            this.filterGroup.OnChannelActive(this);
            return Task.CompletedTask;
        }

        private void OnClientException(Exception ex)
        {
            this.filterGroup.OnChannelException(this, ex);
        }

        private void OnClientClosed(ITcpClient client)
        {
            this.DisposeAsync();
        }

        public override ValueTask DisposeAsync()
        {
            
            this.filterGroup.OnChannelInactive(this);

            return base.DisposeAsync();
        }
    }
}