using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading;

namespace Letter.Udp.Box
{
    public class UdpOptions : Letter.Bootstrap.IOptions
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

        private SchedulerOptions schedulerOptions = new SchedulerOptions(SchedulerType.ThreadPool);
        public SchedulerOptions SchedulerOptions
        {
            get { return schedulerOptions;}
            set
            {
                this.schedulerOptions = value;
                switch (this.schedulerOptions.Type)
                {
                    case SchedulerType.Node:
                        this.SchedulerAllocator = new SchedulerAllocator(this.schedulerOptions.SchedulerCount);
                        break;
                    
                    case SchedulerType.Kestrel:
                        this.SchedulerAllocator = SchedulerAllocator.kestrel;
                        break;
                    case SchedulerType.Processor:
                        this.SchedulerAllocator = SchedulerAllocator.processor;
                        break;
                    case SchedulerType.ThreadPool:
                        this.SchedulerAllocator = SchedulerAllocator.threadPool;
                        break;
                }
            }
        }
        
        internal Func<MemoryPool<byte>> MemoryPoolFactory { get; set; }
        private MemoryPool<byte> OnCreateMemoryPool() => SlabMemoryPoolFactory.Create(this.MemoryPoolOptions);
        
        internal SchedulerAllocator SchedulerAllocator { get; private set; } = SchedulerAllocator.threadPool;
    }
}