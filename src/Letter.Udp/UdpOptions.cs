using System;
using System.Buffers;
using Letter.IO;

namespace Letter.Udp
{
    public class UdpOptions : IOptions
    {
        public virtual int? RcvBufferSize { get; set; }
        public virtual int? SndBufferSize { get; set; }

        public virtual int? RcvTimeout { get; set; }
        public virtual int? SndTimeout { get; set; }

        public BinaryOrder Order { get; set; } = BinaryOrder.BigEndian;

        public MemoryPool<byte> MemoryPool { get; set; } = SlabMemoryPoolFactory.Create();
    }
}