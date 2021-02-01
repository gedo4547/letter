using System;
using System.Buffers;
using System.Collections.Generic;

namespace Letter.Kcp.lib__
{
    class KcpSegmentAllotter : IAllotter<KcpSegment>
    {
        public KcpSegmentAllotter(MemoryPool<byte> memoryPool, bool useLittleEndian)
        {
            this.useLittleEndian = useLittleEndian;
            this.bufferAllotter = new KcpBufferAllotter(memoryPool);
        }

        private bool useLittleEndian;
        private KcpBufferAllotter bufferAllotter;

        private Stack<KcpSegment> bufferStack = new Stack<KcpSegment>();

        public KcpSegment Get()
        {
            if(this.bufferStack.Count > 0)
            {
                return this.bufferStack.Pop();
            }
            KcpBuffer buffer = this.bufferAllotter.Get();
            return new KcpSegment(this.useLittleEndian, buffer);
        }

        public void Put(KcpSegment segment)
        {
            if(segment == null)
            {
                return;
            }

            // segment.Reset();
            this.bufferStack.Push(segment);
        }

        public void Dispose()
        {
            
        }
    }
}
