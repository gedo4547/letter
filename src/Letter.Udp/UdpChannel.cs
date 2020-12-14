using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Udp
{
    sealed class UdpChannel : AChannel<IUdpSession, UdpOptions>, IUdpChannel
    {
        public UdpChannel(SchedulerAllocator allocator, MemoryPool<byte> memoryPool, UdpOptions options, Action<IFilterPipeline<IUdpSession>> handler)
        {
            this.memoryPool = memoryPool;
            this.schedulerAllocator = allocator;
            
            base.ConfigurationSelfOptions(options);
            base.ConfigurationSelfFilter(handler);
            this.filterPipeline = base.CreateFilterPipeline();
        }

        private Socket socket;
        private UdpSession session;
        private MemoryPool<byte> memoryPool;
        private SchedulerAllocator schedulerAllocator;
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
            var scheduler = this.schedulerAllocator.Next();
            this.session = new UdpSession(this.socket, options, this.memoryPool, scheduler, filterPipeline);
            session.Start();
        }

        public async override Task StopAsync()
        {
            await this.session.CloseAsync();
            await base.StopAsync();
        }
    }
}