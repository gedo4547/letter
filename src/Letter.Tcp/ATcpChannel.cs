

using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using FilterGroupFactory = Letter.ChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    abstract class ATcpChannel
    {
        public ATcpChannel(FilterGroupFactory groupFactory, SslFeature sslFeature)
        {
            this.sslFeature = sslFeature;
            this.groupFactory = groupFactory;

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
        private FilterGroupFactory groupFactory;
        protected Func<Socket, ATcpOptions, PipeScheduler, MemoryPool<byte>, ATcpSession> createSession;
        
        private ATcpSession CreateSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool)
        {
            var filterGroup = groupFactory.CreateGroup();
            return new TcpSession(socket, options, scheduler, pool, filterGroup);
        }
        
        private ATcpSession CreateSslSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool)
        {
            var filterGroup = groupFactory.CreateGroup();
            return new TcpSslSession(socket, options, scheduler, pool, this.sslFeature, filterGroup);
        }
    }
}