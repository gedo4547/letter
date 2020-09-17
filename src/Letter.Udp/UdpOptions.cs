using System;
using System.Buffers;
using System.Net.Sockets;
using System.Threading;

namespace Letter.Udp
{
    public class UdpOptions : IOptions
    {
        public int? RcvBufferSize { get; set; }
        public int? SndBufferSize { get; set; }
        
        public int? RcvTimeout { get; set; }
        public int? SndTimeout { get; set; }
        
        public BinaryOrder Order { get; set; } = BinaryOrder.BigEndian;

        public int MemoryBufferLength { get; set; }
        
        public Func<MemoryPool<byte>> MemoryPoolFactory { get; set; } = SlabMemoryPoolFactory.Create;

        public SchedulerAllocator Allocator { get; } = SchedulerAllocator.shared;
    }
}