using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public class UdpClient : ADgramNetwork<UdpOptions, IUdpContext, IUdpChannel>, IUdpClient
    {
        public UdpClient() : base(new UdpOptions())
        {
            this.socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
        }

        private Socket socket;

        public override void Build()
        {
            base.Build();
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                this.socket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            }

            if (this.options.RcvTimeout != null)
                this.socket.SettingRcvTimeout(this.options.RcvTimeout.Value);
            if (this.options.SndTimeout != null)
                this.socket.SettingSndTimeout(this.options.SndTimeout.Value);
            
            if (this.options.RcvBufferSize != null)
                this.socket.SettingRcvBufferSize(this.options.RcvBufferSize.Value);
            if (this.options.SndBufferSize != null)
                this.socket.SettingSndBufferSize(this.options.SndBufferSize.Value);
        }

        public Task StartAsync(EndPoint bindLocalAddress)
        {
            try
            {
                this.socket.Bind(bindLocalAddress);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                throw ex;
            }

            this.Run();
            
            return Task.CompletedTask;
        }

        public async Task StartAsync(EndPoint bindLocalAddress, EndPoint connectRemoteAddress)
        {
            try
            {
                this.socket.Bind(bindLocalAddress);
                await this.socket.ConnectAsync(connectRemoteAddress);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                throw ex;
            }

            this.Run();
        }

        private void Run()
        {
            var channelGroup = this.ChannelGroupFactory.CreateChannelGroup();
            UdpContext context = new UdpContext(channelGroup, this.options.MemoryPool);
            
            context.Start(this.socket);
        }
    }
}