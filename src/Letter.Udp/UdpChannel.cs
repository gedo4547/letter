using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Udp
{
    sealed class UdpChannel : IUdpChannel
    {
        public UdpChannel(UdpOptions options, Action<IFilterPipeline<IUdpSession>> handler)
        {
            this.options = options;
            this.filterPipeline = new FilterPipeline<IUdpSession>();
            if (handler != null) handler(this.filterPipeline);
        }

        private Socket socket;
        private UdpOptions options;
        private FilterPipeline<IUdpSession> filterPipeline;
        
        public EndPoint BindAddress { get; private set; }
        
        public Task StartAsync(EndPoint bindAddress)
        {
            this.Bind(bindAddress);
            
            return Task.CompletedTask;
        }
        
        public async Task StartAsync(EndPoint bindAddress, EndPoint connectAddress)
        {
            this.Bind(bindAddress);

            await this.socket.ConnectAsync(connectAddress);
        }
        
        private void Bind(EndPoint bindAddress)
        {
            this.socket = new Socket(bindAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                this.socket.Bind(bindAddress);
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                throw new AddressInUseException(e.Message, e);
            }

            this.BindAddress = this.socket.LocalEndPoint;
        }

        private void Run()
        {
            var memoryPool = this.options.MemoryPoolFactory();
            var scheduler = this.options.SchedulerAllocator.Next();
            UdpSession session = new UdpSession(this.socket, options, memoryPool, scheduler, filterPipeline);
            session.Start();
        }

        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}