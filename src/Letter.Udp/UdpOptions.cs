using System;
using System.Buffers;
using System.Threading;

namespace Letter.Udp
{
    public class UdpOptions : IOptions
    {
        public UdpOptions()
        {
            this.MemoryPoolFactory = this.OnCreateMemoryPool;
        }
        
        public int? RcvBufferSize { get; set; }
        public int? SndBufferSize { get; set; }
        public int? RcvTimeout { get; set; }
        public int? SndTimeout { get; set; }
        public BinaryOrder Order { get; set; } = BinaryOrder.BigEndian;
        
        public MemoryPoolOptions MemoryPoolOptions { get; set; } = new MemoryPoolOptions()
        {
            MemoryBlockSize = 4096, 
            MemoryBlockCount = 32
        };

        public int SchedulerCount
        {
            get { return SchedulerAllocator.Count;}
            set
            {
                int count = value;
                if (count == SchedulerAllocator.Count)
                    return;
                this.SchedulerAllocator = new SchedulerAllocator(count);
            }
        }
        
        private MemoryPool<byte> OnCreateMemoryPool()
        {
            return SlabMemoryPoolFactory.Create(this.MemoryPoolOptions);
        }

        internal Func<MemoryPool<byte>> MemoryPoolFactory { get; set; }
        internal SchedulerAllocator SchedulerAllocator { get; private set; } = SchedulerAllocator.threadPool;
    }
}