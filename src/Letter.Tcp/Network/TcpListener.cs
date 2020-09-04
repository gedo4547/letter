using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpListener : ATcpNetwork<TcpServerOptions>, ITcpListener
    {
        public TcpListener() : base(new TcpServerOptions())
        {
        }

        private Socket listenSocket;
        private SafeSocketHandle socketHandle;
        private MemoryPool<byte> memoryPool;
        
        private int numSchedulers;
        private int schedulerIndex;
        private PipeScheduler[] schedulers;

        public EndPoint EndPoint { get; private set; }
        
        public override void Build()
        {
            base.Build();
            
            this.memoryPool = this.options.MemoryPoolFactory();
            var ioQueueCount = options.IOQueueCount;

            if (ioQueueCount > 0)
            {
                numSchedulers = ioQueueCount;
                schedulers = new IOQueue[numSchedulers];

                for (var i = 0; i < numSchedulers; i++)
                {
                    schedulers[i] = new IOQueue();
                }
            }
            else
            {
                var directScheduler = new PipeScheduler[] { PipeScheduler.ThreadPool };
                numSchedulers = directScheduler.Length;
                schedulers = directScheduler;
            }
        }

        public void Bind(EndPoint point)
        {
            if (this.listenSocket != null)
            {
                throw new InvalidOperationException(SocketsStrings.TransportAlreadyBound);
            }

            EndPoint = point;
            Socket listenSocket;

            switch (EndPoint)
            {
                case FileHandleEndPoint fileHandle:
                    this.socketHandle = new SafeSocketHandle((IntPtr)fileHandle.FileHandle, ownsHandle: true);
                    listenSocket = new Socket(this.socketHandle);
                    break;
                case UnixDomainSocketEndPoint unix:
                    listenSocket = new Socket(unix.AddressFamily, SocketType.Stream, ProtocolType.Unspecified);
                    BindSocket();
                    break;
                case IPEndPoint ip:
                    listenSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    // Kestrel expects IPv6Any to bind to both IPv6 and IPv4
                    if (ip.Address == IPAddress.IPv6Any)
                    {
                        listenSocket.DualMode = true;
                    }
                    BindSocket();
                    break;
                default:
                    listenSocket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    BindSocket();
                    break;
            }

            void BindSocket()
            {
                try
                {
                    listenSocket.Bind(EndPoint);
                }
                catch (SocketException e) when (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    throw new AddressInUseException(e.Message, e);
                }
            }

            EndPoint = listenSocket.LocalEndPoint;

            listenSocket.Listen(this.options.Backlog);

            this.listenSocket = listenSocket;
        }

        public async ValueTask<ITcpSession> AcceptAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                try
                {
                    var acceptSocket = await this.listenSocket.AcceptAsync();

                    // Only apply no delay to Tcp based endpoints
                    if (acceptSocket.LocalEndPoint is IPEndPoint)
                    {
                        acceptSocket.NoDelay = this.options.NoDelay;
                    }
                    
                    var session = new TcpSession(
                        memoryPool, 
                        schedulers[schedulerIndex],
                        trace,
                        options.MaxReadBufferSize, 
                        options.MaxWriteBufferSize, 
                        options.WaitForDataBeforeAllocatingBuffer);
                    
                    session.Start(acceptSocket);
                    
                    this.schedulerIndex = (this.schedulerIndex + 1) % this.numSchedulers;

                    return session;
                }
                catch (ObjectDisposedException)
                {
                    // A call was made to UnbindAsync/DisposeAsync just return null which signals we're done
                    return null;
                }
                catch (SocketException e) when (e.SocketErrorCode == SocketError.OperationAborted)
                {
                    // A call was made to UnbindAsync/DisposeAsync just return null which signals we're done
                    return null;
                }
                catch (SocketException)
                {
                    // The connection got reset while it was in the backlog, so we try again.
                    this.trace.ConnectionReset(connectionId: "(null)");
                }
            }
        }

        public ValueTask UnbindAsync(CancellationToken cancellationToken = default)
        {
            this.listenSocket?.Dispose();

            this.socketHandle?.Dispose();
            
            return default;
        }

        public override Task StopAsync()
        {
            this.listenSocket?.Dispose();

            this.socketHandle?.Dispose();

            // Dispose the memory pool
            this.memoryPool.Dispose();
            
            
            return base.StopAsync();
        }
    }
}