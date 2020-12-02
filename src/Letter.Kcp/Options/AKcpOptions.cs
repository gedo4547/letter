using System;
using System.Buffers;
using System.Buffers.Binary;
using Letter.IO;

namespace Letter.Kcp
{
    public abstract class AKcpOptions : IOptions
    {
        public int? RcvBufferSize { get; set; }
        public int? SndBufferSize { get; set; }
        public int? RcvTimeout { get; set; }
        public int? SndTimeout { get; set; }
        public BinaryOrder Order { get; set; } = BinaryOrder.BigEndian;
        public MemoryPoolOptions MemoryPoolOptions { get; set; } = new MemoryPoolOptions(4096, 32);
        public int SchedulerCount { get; set; } = Math.Min(Environment.ProcessorCount, 16);
    }
}