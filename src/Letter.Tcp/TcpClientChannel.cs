using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    class TcpClientChannel : ATcpChannel, ITcpClientChannel
    {
        public TcpClientChannel(SchedulerAllocator allocator, MemoryPool<byte> memoryPool, TcpClientOptions options,
            Action<IFilterPipeline<ITcpSession>> handler, SslFeature sslFeature) 
        
            : base(handler, sslFeature)
        {
            this.options = options;
            this.memoryPool = memoryPool;
            this.schedulerAllocator = allocator;
        }
        
        private Socket connectSocket;
        private TcpClientOptions options;
        private MemoryPool<byte> memoryPool;
        private SchedulerAllocator schedulerAllocator;

        private ATcpSession session;
        
        public EndPoint ConnectAddress { get; private set; }

        public async Task StartAsync(EndPoint address)
        {
            if (this.connectSocket == null)
            {
                this.connectSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            
            await this.connectSocket.ConnectAsync(address);
            this.ConnectAddress = this.connectSocket.LocalEndPoint;

            var scheduler = this.schedulerAllocator.Next();
            this.session = this.createSession(this.connectSocket, this.options, scheduler, this.memoryPool);
            
            await this.session.StartAsync();
        }
        
        public Task StopAsync()
        {
            return Task.CompletedTask;
        }
    }
}