using System;
using System.Buffers;
using Letter.IO;

namespace Letter.Udp
{
    public class UdpOptions : IOptions
    {
        public virtual int RcvBufferSize { get; set; } = -1;
        public virtual int SndBufferSize { get; set; } = -1;

        public int RcvTimeout { get; set; } = -1;
        public int SndTimeout { get; set; } = -1;

        public BinaryOrder Order { get; set; } = BinaryOrder.BigEndian;

        public MemoryPool<byte> MemoryPool { get; set; } = SlabMemoryPoolFactory.Create();
    }
}