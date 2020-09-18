using System;
using FilterGroupFactory = Letter.StreamChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    abstract class ATcpChannel : IDisposable
    {
        public ATcpChannel(FilterGroupFactory groupFactory, SslFeature sslFeature)
        {
            this.sslFeature = sslFeature;
            this.groupFactory = groupFactory;

            if(sslFeature == null)
                this.sessionCreator = this.CreateTcpSession;
            else
                this.sessionCreator = this.CreateSslTcpSession;
        }

        protected SslFeature sslFeature;
        protected FilterGroupFactory groupFactory;
        protected Func<ITcpClient, IInternalTcpSession> sessionCreator;

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

        public void Dispose()
        {
            this.sslFeature = null;
            this.groupFactory = null;
            this.sessionCreator = null;
        }
    }
}