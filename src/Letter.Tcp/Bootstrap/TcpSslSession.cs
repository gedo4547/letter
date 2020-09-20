using System.Threading.Tasks;

using FilterGroup = Letter.StreamChannelFilterGroup<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    class TcpSslSession : TcpSession
    {
        public TcpSslSession(ITcpClient client, SslStreamDuplexPipe transport, SslFeature sslFeature, FilterGroup filterGroup) :
            base(client, transport, filterGroup)
        {
            this.sslFeature = sslFeature;
            this.sslDuplexPipe = transport;
        }

        private SslFeature sslFeature;
        private SslStreamDuplexPipe sslDuplexPipe;
        
        public override async Task StartAsync()
        {
            var tlsOptions = sslFeature.sslOptions;
            switch (tlsOptions)
            {
                case SslServerOptions serverTlsOptions:
                    await this.sslDuplexPipe.Stream.AuthenticateAsServerAsync(
                        serverTlsOptions.Certificate,
                        serverTlsOptions.NegotiateClientCertificate,
                        serverTlsOptions.EnabledProtocols,
                        serverTlsOptions.CheckCertificateRevocation);
                    break;
                case SslClientOptions clientTlsOptions:
                    await this.sslDuplexPipe.Stream.AuthenticateAsClientAsync(
                        clientTlsOptions.TargetHost,
                        clientTlsOptions.X509CertificateCollection,
                        clientTlsOptions.EnabledProtocols,
                        clientTlsOptions.CheckCertificateRevocation);
                    break;
            }
            
            
            await base.StartAsync();
        }
    }
}