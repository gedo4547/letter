using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    class TcpClientBootstrap : ATcpBootstrap<TcpClientOptions, ITcpClientChannel>, ITcpClientBootstrap
    {
        private MemoryPool<byte> memoryPool;
        private SchedulerAllocator schedulerAllocator;
        
        public override async Task BuildAsync()
        {
            await base.BuildAsync();
            this.memoryPool= SlabMemoryPoolFactory.Create(this.options.MemoryPoolOptions);
            this.schedulerAllocator = new SchedulerAllocator(this.options.SchedulerCount);
        }

        protected override Task<ITcpClientChannel> ChannelFactoryAsync(TcpClientOptions options, Action<IFilterPipeline<ITcpSession>> handler, SslFeature sslFeature)
        {
            return Task.FromResult((ITcpClientChannel) new TcpClientChannel(this.schedulerAllocator, this.memoryPool, options, handler, sslFeature));
        }

        public override ValueTask DisposeAsync()
        {
            this.memoryPool.Dispose();
            return base.DisposeAsync();
        }
    }
}