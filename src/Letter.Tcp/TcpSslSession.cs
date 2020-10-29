using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpSslSession : TcpSession
    {
        public TcpSslSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool, SslFeature sslFeature, FilterPipeline<ITcpSession> filterPipeline)
            : base(socket, options, scheduler, pool, filterPipeline)
        {
            var inputPipeOptions = StreamPipeOptionsHelper.ReaderOptionsCreator(pool);
            var outputPipeOptions = StreamPipeOptionsHelper.WriterOptionsCreator(pool);
            var sslDuplexPipe = new SslStreamDuplexPipe(base.Transport, inputPipeOptions, outputPipeOptions, sslFeature.sslStreamFactory);
            
            this.sslTransport = sslDuplexPipe;
            this.sslOptions = sslFeature.sslOptions;
        }
        
        protected override StreamPipelineReader Input
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.sslTransport.WrappedInput; }
        }

        protected override StreamPipelineWriter Output
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.sslTransport.WrappedOutput; }
        }

        private SslOptions sslOptions;
        private SslStreamDuplexPipe sslTransport;
        
        public async override Task StartAsync()
        {
            var tlsOptions = this.sslOptions;
            var sslDuplexPipe = this.sslTransport;
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

        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }
    }
}