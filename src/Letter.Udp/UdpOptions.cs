﻿using System;
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
        
        public MemoryOptions MemoryOptions { get; set; } = new MemoryOptions()
        {
            MemoryBlockSize = 4096, 
            MemoryBlockCount = 32
        };

        public int SchedulerCount
        {
            get { return SchedulerAllocator.shared.Count;}
            set
            {
                int count = value;
                if (count == SchedulerAllocator.shared.Count)
                    return;
                this.Allocator = new SchedulerAllocator(count);
            }
        }
        
        private MemoryPool<byte> OnCreateMemoryPool()
        {
            return SlabMemoryPoolFactory.Create(this.MemoryOptions);
        }

        internal Func<MemoryPool<byte>> MemoryPoolFactory { get; set; }
        internal SchedulerAllocator Allocator { get; private set; } = SchedulerAllocator.shared;
    }
}