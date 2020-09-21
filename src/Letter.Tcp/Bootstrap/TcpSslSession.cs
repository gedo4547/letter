using System.Threading.Tasks;

using FilterGroup = Letter.StreamChannelFilterGroup<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    class TcpSslSession : TcpSession
    {
        public TcpSslSession(ITcpClient client, SslStreamDuplexPipe transport, SslOptions sslOptions, FilterGroup filterGroup) :
            base(client, transport, filterGroup)
        {
            this.sslOptions = sslOptions;
        }

        private SslOptions sslOptions;

        public override async Task StartAsync()
        {
            var tlsOptions = this.sslOptions;
            var sslDuplexPipe = this.transport as SslStreamDuplexPipe;
            switch (tlsOptions)
            {
                case SslServerOptions serverTlsOptions:
                    await sslDuplexPipe.Stream.AuthenticateAsServerAsync(
                        serverTlsOptions.Certificate,
                        serverTlsOptions.NegotiateClientCertificate,
                        serverTlsOptions.EnabledProtocols,
                        serverTlsOptions.CheckCertificateRevocation);
                    break;
                case SslClientOptions clientTlsOptions:
                    await sslDuplexPipe.Stream.AuthenticateAsClientAsync(
                        clientTlsOptions.TargetHost,
                        clientTlsOptions.X509CertificateCollection,
                        clientTlsOptions.EnabledProtocols,
                        clientTlsOptions.CheckCertificateRevocation);
                    break;
            }
            
            await base.StartAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            if (this.isDispose)
            {
                return;
            }

            this.isDispose = true;
            
            var sslDuplexPipe = this.transport as SslStreamDuplexPipe;
            await sslDuplexPipe.DisposeAsync();
            
            await this.readBufferTask;
            await this.client.DisposeAsync();
        }
    }
}