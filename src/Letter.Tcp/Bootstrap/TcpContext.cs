using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    public partial class TcpContext : ITcpContext
    {
        public static TcpContext Get(List<ITcpChannel> channels, BinaryOrder order)
        {
            return null;
        }

        public TcpContext(TcpChannelGroup channelGroup, BinaryOrder order)
        {
            if (channelGroup == null)
            {
                throw new ArgumentNullException(nameof(channelGroup));
            }

            this.order = order;
            this.channelGroup = channelGroup;
        }

        public string Id => this.client.Id;
        public EndPoint LoaclAddress => this.client.LocalAddress;
        public EndPoint RemoteAddress => this.client.RemoteAddress;
        public MemoryPool<byte> MemoryPool => this.client.MemoryPool;
        

        private BinaryOrder order;

        private ITcpClient client;
        private TcpChannelGroup channelGroup;
        
        public void Initialize(ITcpClient client)
        {
            this.client = client;
            
            this.ReaderMemoryPolledIOAsync().NoAwait();

            this.channelGroup.OnChannelActive(this);
        }

        public Task WriteAsync(object o)
        {
            this.SenderMemoryIOAsync(o).NoAwait();
            
            return Task.CompletedTask;
        }

        public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            var sequence = new ReadOnlySequence<byte>(buffer, offset, count);

            this.SenderMemoryIOAsync(ref sequence).NoAwait();
            
            return Task.CompletedTask;
        }
        
        public async Task CloseAsync()
        {
            await this.client.CloseAsync();
        }
        
        public async ValueTask DisposeAsync()
        {
            this.channelGroup.Dispose();
            await this.client.DisposeAsync();
        }
    }
}