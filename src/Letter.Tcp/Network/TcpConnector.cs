using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpConnector : ATcpNetwork<TcpConnectorOptions>, ITcpConnector
    {
        public TcpConnector(TcpConnectorOptions options) : base(options)
        {
        }
        
        private MemoryPool<byte> memoryPool;

        public override void Build()
        {
            base.Build();
            
            this.memoryPool = options.MemoryPoolFactory();
        }

        public async ValueTask<ITcpSession> ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
        {
            var ipEndPoint = endpoint as IPEndPoint;

            if (ipEndPoint is null)
            {
                throw new NotSupportedException("The SocketConnectionFactory only supports IPEndPoints for now.");
            }

            var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = this.options.NoDelay
            };

            await socket.ConnectAsync(ipEndPoint);

            var session = new TcpSession(
                socket,
                this.memoryPool,
                PipeScheduler.ThreadPool,
                this.trace,
                this.options.MaxReadBufferSize,
                this.options.MaxWriteBufferSize,
                this.options.WaitForDataBeforeAllocatingBuffer);

            session.Start();
            
            return session;
        }

        public override Task StopAsync()
        {
            this.memoryPool.Dispose();
            this.memoryPool = null;
            
            return base.StopAsync();
        }
    }
}