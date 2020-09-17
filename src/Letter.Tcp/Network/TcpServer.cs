using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpServer : ATcpNetwork<TcpServerOptions>, ITcpServer
    {
        public TcpServer() : base(new TcpServerOptions())
        {
        }

        public EndPoint BindAddress { get; private set; }
        
        private Socket listenSocket;
        private SafeSocketHandle socketHandle;
        private MemoryPool<byte> memoryPool;
        
        private int numSchedulers;
        private int schedulerIndex;
        private PipeScheduler[] schedulers;

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

            this.BindAddress = point;
            Socket listenSocket;
            switch (this.BindAddress)
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

        public async ValueTask<ITcpClient> AcceptAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                try
                {
                    var acceptSocket = await this.listenSocket.AcceptAsync();
                    if (acceptSocket.LocalEndPoint is IPEndPoint)
                    {
                        acceptSocket.NoDelay = this.options.NoDelay;
                    }
                    
                    TcpClient client = new TcpClient();
                    client.ConfigureOptions(this.OnConfigureClientOptions);
                    client.Build();
                    client.Start(acceptSocket, this.schedulers[this.schedulerIndex]);
                    
                    this.schedulerIndex = (this.schedulerIndex + 1) % this.numSchedulers;

                    return client;
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

        private void OnConfigureClientOptions(TcpClientOptions options)
        {
            options.WaitForDataBeforeAllocatingBuffer = this.options.WaitForDataBeforeAllocatingBuffer;
            options.MaxPipelineReadBufferSize = this.options.MaxPipelineReadBufferSize;
            options.MaxPipelineWriteBufferSize = this.options.MaxPipelineWriteBufferSize;
            options.MemoryPoolFactory = this.ClientMemoryPoolFactory;
        }
        
        private MemoryPool<byte> ClientMemoryPoolFactory() => this.memoryPool;
        
        public override ValueTask DisposeAsync()
        {
            if (this.listenSocket != null)
            {
                this.listenSocket.Dispose();
                this.listenSocket = null;
            }

            if (this.socketHandle != null)
            {
                this.socketHandle.Dispose();
                this.socketHandle = null;
            }

            this.memoryPool.Dispose();
            this.memoryPool = null;
            return default;
        }
    }
}