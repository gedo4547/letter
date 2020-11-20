using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Udp
{
    sealed class UdpBootstrap : ABootstrap<UdpOptions, IUdpSession, IUdpChannel>, IUdpBootstrap
    {
        private MemoryPool<byte> memoryPool;
        private SchedulerAllocator schedulerAllocator;
        
        public override async Task BuildAsync()
        {
            await base.BuildAsync();
            this.memoryPool= SlabMemoryPoolFactory.Create(this.options.MemoryPoolOptions);
            this.schedulerAllocator = new SchedulerAllocator(this.options.SchedulerCount);
        }
        
        protected override Task<IUdpChannel> ChannelFactoryAsync(UdpOptions options, Action<IFilterPipeline<IUdpSession>> handler)
        {
            UdpChannel channel = new UdpChannel(this.schedulerAllocator, this.memoryPool, options, handler);
            return Task.FromResult((IUdpChannel) channel);
        }
    }
}