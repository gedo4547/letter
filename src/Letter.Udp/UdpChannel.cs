using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using FilterGroupFactory = Letter.DgramChannelFilterGroupFactory<Letter.Udp.IUdpSession, Letter.Udp.IUdpChannelFilter>;

namespace Letter.Udp
{
    public class UdpChannel : IUdpChannel
    {
        public UdpChannel(UdpOptions options, FilterGroupFactory groupFactory)
        {
            this.options = options;
            this.groupFactory = groupFactory;
        }

        private Socket socket;
        private UdpSession session;
        
        private UdpOptions options;
        private FilterGroupFactory groupFactory;

        private string name;
        
        public async Task StartAsync(EndPoint bindAddress, EndPoint connectAddress, string name)
        {
            await this.StartAsync(bindAddress, name);

            await this.socket.ConnectAsync(connectAddress);
            
            this.Run();
        }
        
        public Task StartAsync(EndPoint bindAddress, string name)
        {
            this.name = name;
            this.CreateSocket(bindAddress.AddressFamily);
            
            try
            {
                socket.Bind(bindAddress);
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                throw new AddressInUseException(e.Message, e);
            }
            
            this.Run();
            
            return Task.CompletedTask;
        }
        
        private void CreateSocket(AddressFamily family)
        {
            this.socket = new Socket(family, SocketType.Dgram, ProtocolType.Udp);
            
            if (this.options.RcvTimeout != null)
                this.socket.SettingRcvTimeout(this.options.RcvTimeout.Value);
            if (this.options.SndTimeout != null)
                this.socket.SettingSndTimeout(this.options.SndTimeout.Value);
            
            if (this.options.RcvBufferSize != null)
                this.socket.SettingRcvBufferSize(this.options.RcvBufferSize.Value);
            if (this.options.SndBufferSize != null)
                this.socket.SettingSndBufferSize(this.options.SndBufferSize.Value);
        }

        private void Run()
        {
            var filterGroup = this.groupFactory.CreateFilterGroup();
            var memoryPool = this.options.MemoryPoolFactory();
            PipeScheduler scheduler = this.options.Allocator.Next();
            
            this.session = new UdpSession(this.options.Order, memoryPool, scheduler, filterGroup);
            this.session.StartAsync(this.socket, name);
        }
        
        public async ValueTask DisposeAsync()
        {
            if (this.session != null)
            {
                await this.session.DisposeAsync();
                this.socket = null;
            }

            this.options = null;
            this.groupFactory = null;
        }
    }
}