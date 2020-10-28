using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace Letter.Tcp
{
    class TcpSslSession : ATcpSession
    {
        public TcpSslSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool, SslFeature sslFeature, FilterPipeline<ITcpSession> filterPipeline)
            : base(socket, options, scheduler, pool, filterPipeline)
        {
            var inputPipeOptions = StreamPipeOptionsHelper.ReaderOptionsCreator(pool);
            var outputPipeOptions = StreamPipeOptionsHelper.WriterOptionsCreator(pool);
            var sslDuplexPipe = new SslStreamDuplexPipe(
                base.Transport, 
                inputPipeOptions, 
                outputPipeOptions, 
                sslFeature.sslStreamFactory);
            this.SslTransport = sslDuplexPipe;
            
            this.sslOptions = sslFeature.sslOptions;
        }

        private IWrappedDuplexPipe SslTransport
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        private SslOptions sslOptions;
        
        public async override Task StartAsync()
        {
            var tlsOptions = this.sslOptions;
            var sslDuplexPipe = this.SslTransport as SslStreamDuplexPipe;
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
            
            
        }

        public override Task WriteAsync(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}