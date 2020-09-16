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
        public static TcpContext Get(List<ITcpFilter> Filters, BinaryOrder order)
        {
            return null;
        }

        public TcpContext(TcpFilterGroup FilterGroup, BinaryOrder order)
        {
            if (FilterGroup == null)
            {
                throw new ArgumentNullException(nameof(FilterGroup));
            }

            this.order = order;
            this.FilterGroup = FilterGroup;
        }

        public string Id => this.client.Id;
        public EndPoint LoaclAddress => this.client.LocalAddress;
        public EndPoint RemoteAddress => this.client.RemoteAddress;
        public MemoryPool<byte> MemoryPool => this.client.MemoryPool;
        

        private BinaryOrder order;

        private ITcpClient client;
        private TcpFilterGroup FilterGroup;
        
        public void Initialize(ITcpClient client)
        {
            this.client = client;
            
            this.ReaderMemoryPolledIOAsync().NoAwait();

            this.FilterGroup.OnFilterActive(this);
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
            this.FilterGroup.Dispose();
            await this.client.DisposeAsync();
        }
    }
}