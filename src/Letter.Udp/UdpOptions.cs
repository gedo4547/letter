using System.Buffers;
using Letter;

namespace Letter.Udp
{
    public class UdpOptions : IOptions
    {
        public int? RcvBufferSize { get; set; }
        public int? SndBufferSize { get; set; }
        
        public int? RcvTimeout { get; set; }
        public int? SndTimeout { get; set; }
        
        public BinaryOrder Order { get; set; } = BinaryOrder.BigEndian;
        
        public MemoryPool<byte> MemoryPool { get; set; }
    }
}