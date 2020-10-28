using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;

namespace Letter.Tcp
{
    abstract class ATcpChannel
    {
        public ATcpChannel(Action<IFilterPipeline<ITcpSession>> handler, SslFeature sslFeature)
        {
            this.sslFeature = sslFeature;
            this.handler = handler;

            if (sslFeature == null)
            {
                this.createSession = CreateSession;
            }
            else
            {
                this.createSession = CreateSslSession;
            }
        }
        
        private SslFeature sslFeature;
        private Action<IFilterPipeline<ITcpSession>> handler;
        protected Func<Socket, ATcpOptions, PipeScheduler, MemoryPool<byte>, ATcpSession> createSession;
        
        private ATcpSession CreateSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool)
        {
            return new TcpSession(socket, options, scheduler, pool, this.CreateFilterPipeline());
        }
        
        private ATcpSession CreateSslSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool)
        {
            return new TcpSslSession(socket, options, scheduler, pool, this.sslFeature, this.CreateFilterPipeline());
        }

        private FilterPipeline<ITcpSession> CreateFilterPipeline()
        {
            FilterPipeline<ITcpSession> filterPipeline = new FilterPipeline<ITcpSession>();
            if (this.handler != null)
            {
                this.handler(filterPipeline);
            }

            return filterPipeline;
        }
    }
}