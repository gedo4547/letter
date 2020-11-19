using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    class TcpServerChannel : ATcpChannel, ITcpServerChannel
    {
        public TcpServerChannel(TcpServerOptions options, Action<IFilterPipeline<ITcpSession>> handler, SslFeature sslFeature)
            : base(handler, sslFeature)
        {
            this.options = options;
            this.memoryPool = SlabMemoryPoolFactory.Create(this.options.MemoryPoolOptions);
            this.schedulerAllocator = new SchedulerAllocator(this.options.SchedulerCount);
        }
        
        private Socket listenSocket;

        private TcpServerOptions options;
        private MemoryPool<byte> memoryPool;
        private SchedulerAllocator schedulerAllocator;
        
        private Task acceptTask;

        public EndPoint BindAddress { get; private set; }


        public Task StartAsync(EndPoint address)
        {
            this.Bind(address);
            this.acceptTask = this.LoopAcceptAsync();
            return Task.CompletedTask;
        }
        
        public void Bind(EndPoint point)
        {
            if (this.listenSocket != null)
            {
                throw new InvalidOperationException(SocketsStrings.TransportAlreadyBound);
            }

            this.BindAddress = point;
            Socket listenSocket;
            switch (this.BindAddress)
            {
                case IPEndPoint ip:
                    listenSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    if (ip.Address == IPAddress.IPv6Any) listenSocket.DualMode = true;
                    BindSocket();
                    break;
                default:
                    listenSocket = new Socket(this.BindAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    BindSocket();
                    break;
            }

            void BindSocket()
            {
                try
                {
                    listenSocket.Bind(this.BindAddress);
                }
                catch (SocketException e) when (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    throw new AddressInUseException(e.Message, e);
                }
            }

            this.BindAddress = listenSocket.LocalEndPoint;
            listenSocket.Listen(this.options.Backlog);
            this.listenSocket = listenSocket;
        }

        private async Task LoopAcceptAsync()
        {
            while (true)
            {
                var acceptSocket = await this.AcceptAsync();
                if (acceptSocket == null)
                {
                    break;
                }
                var scheduler = this.schedulerAllocator.Next();
                var session = this.createSession(acceptSocket, options, scheduler, this.memoryPool);
                
                await session.StartAsync();
            }
        }

        private async ValueTask<Socket> AcceptAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                try
                {
                    return await this.listenSocket.AcceptAsync();
                }
                catch (ObjectDisposedException)
                {
                    return null;
                }
                catch (SocketException e) when (e.SocketErrorCode == SocketError.OperationAborted)
                {
                    return null;
                }
                catch (SocketException)
                {
                    // The connection got reset while it was in the backlog, so we try again.
                }
            }
        }
        
        public async ValueTask DisposeAsync()
        {
            //if (this.socketHandle != null)
            //{
            //    this.socketHandle.Dispose();
            //}

            if (this.listenSocket != null)
            {
                this.listenSocket.Dispose();
            }
            
            await acceptTask;

            if (this.memoryPool != null)
            {
                this.memoryPool.Dispose();
            }
        }
    }
}