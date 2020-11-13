using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
        private UdpSession session;
        private UdpOptions options;
        private FilterPipeline<IUdpSession> filterPipeline;
        
        public EndPoint BindAddress { get; private set; }
        
        public Task StartAsync(EndPoint bindAddress)
        {
            this.Bind(bindAddress);
            
            this.Run();
            
            return Task.CompletedTask;
        }
        
        public async Task StartAsync(EndPoint bindAddress, EndPoint connectAddress)
        {
            this.Bind(bindAddress);

            await this.socket.ConnectAsync(connectAddress);

            this.Run();
        }
        
        private void Bind(EndPoint bindAddress)
        {
            this.socket = new Socket(bindAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                this.socket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            }

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
            this.session = new UdpSession(this.socket, options, memoryPool, scheduler, filterPipeline);
            session.Start();
        }

        public async ValueTask DisposeAsync()
        {
            await this.session.DisposeAsync();
        }
    }
}