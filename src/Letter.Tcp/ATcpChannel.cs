using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using Letter.IO;

namespace Letter.Tcp
{
    abstract class ATcpChannel<TOptions> : AChannel<ITcpSession, TOptions>
        where TOptions : ATcpOptions
    {
        public ATcpChannel(Action<IFilterPipeline<ITcpSession>> handler, SslFeature sslFeature, TOptions options)
        {
            base.ConfigurationSelfOptions(options);
            base.ConfigurationSelfFilter(handler);
            this.sslFeature = sslFeature;

            if (sslFeature == null)
                this.createSession = CreateSession;
            else
                this.createSession = CreateSslSession;
        }
        
        private SslFeature sslFeature;
        protected Func<Socket, ATcpOptions, PipeScheduler, MemoryPool<byte>, ATcpSession> createSession;
        
        private ATcpSession CreateSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool)
        {
            return new TcpSession(socket, options, scheduler, pool, this.CreateFilterPipeline());
        }
        
        private ATcpSession CreateSslSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool)
        {
            return new TcpSslSession(socket, options, scheduler, pool, this.sslFeature, this.CreateFilterPipeline());
        }
    }
}