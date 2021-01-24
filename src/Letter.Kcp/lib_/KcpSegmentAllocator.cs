using System.Buffers;
using System.Collections.Generic;

namespace KcpProject
{
    class KcpSegmentAllocator
    {
        public KcpSegmentAllocator(MemoryPool<byte> memoryPool, bool isLittleEndian)
        {
            this.isLittleEndian = isLittleEndian;
            this.bufferAllocator = new KcpBufferAllocator(memoryPool);
        }

        private bool isLittleEndian;
        private KcpBufferAllocator bufferAllocator;
        private Stack<KcpSegment> segments = new Stack<KcpSegment>(32);

        public KcpSegment Get(int size)
        {
            if (this.segments.Count > 0)
            {
                return segments.Pop();
            }

            var buffer = this.bufferAllocator.Get(size);
            return KcpSegment.Get(size, this.isLittleEndian, buffer);
        }

        public void Put(KcpSegment segment)
        {
            if (segment == null)
            {
                return;
            }
            
            this.segments.Push(segment);
        }
    }
}