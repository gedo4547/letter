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
        public TcpContext(List<ITcpChannel> channels, BinaryOrder order)
        {
            if (channels == null)
            {
                throw new ArgumentNullException(nameof(channels));
            }

            this.order = order;
            this.channels = channels;
        }
        
        public string Id
        {
            get { return this.client.Id; }
        }
        
        public EndPoint LoaclAddress 
        {
            get { return this.client.LocalAddress; }
        }
        
        public EndPoint RemoteAddress 
        {
            get { return this.client.RemoteAddress; }
        }
        
        public MemoryPool<byte> MemoryPool 
        {
            get { return this.client.MemoryPool; }
        }

        private BinaryOrder order;

        private ITcpClient client;
        private List<ITcpChannel> channels;
        
        public void Initialize(ITcpClient client)
        {
            this.client = client;
            
            this.OnTransportActive();
        }

        public Task WriteAsync(object o)
        {
            this.SenderMemoryIOAsync(o).ConfigureAwait(false);
            
            return Task.CompletedTask;
        }

        public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(buffer, offset, count);

            this.SenderMemoryIOAsync(ref sequence).ConfigureAwait(false);
            
            return Task.CompletedTask;
        }
        
        public async Task CloseAsync()
        {
            this.channels.Clear();
            await this.client.CloseAsync();
        }
        
        public async ValueTask DisposeAsync()
        {
            await this.client.DisposeAsync();
        }
    }
}