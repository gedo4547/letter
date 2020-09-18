using System;
using System.Buffers;
using System.Net.Sockets;
using System.Threading;

namespace Letter.Tcp
{
    public abstract class ATcpOptions : IOptions
    {
        public ATcpOptions()
        {
            this.MemoryPoolFactory = this.OnCreateMemoryPool;
        }

        /// <summary>
        /// Wait until there is data available to allocate a buffer. Setting this to false can increase throughput at the cost of increased memory usage.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public bool WaitForDataBeforeAllocatingBuffer { get; set; } = true;

        /// <summary>
        /// Set to false to enable Nagle's algorithm for all connections.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public bool NoDelay { get; set; } = true;
        public bool KeepAlive { get; set; } = false;
        public int? RcvBufferSize { get; set; }
        public int? SndBufferSize { get; set; }
        public int? RcvTimeout { get; set; }
        public int? SndTimeout { get; set; }
        public LingerOption LingerOption { get; } = new LingerOption(false, 0);
        
        public long? MaxPipelineReadBufferSize { get; set; } = 1024 * 1024;
        public long? MaxPipelineWriteBufferSize { get; set; } = 64 * 1024;
        
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