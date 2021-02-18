using System;
using System.Buffers;
using System.Collections.Generic;

namespace Letter.Kcp.lib__
{
    class KcpSegmentAllotter : IKcpAllotter<KcpSegment>
    {
        public KcpSegmentAllotter(MemoryPool<byte> memoryPool, bool useLittleEndian)
        {
            this.useLittleEndian = useLittleEndian;
            this.bufferAllotter = new KcpBufferAllotter(memoryPool);
        }

        private bool useLittleEndian;
        private KcpBufferAllotter bufferAllotter;

        private Stack<KcpSegment> segmentStack = new Stack<KcpSegment>();

        public KcpSegment Get()
        {
            if (this.segmentStack.Count > 0)
            {
                return this.segmentStack.Pop();
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

            segment.Reset();
            this.segmentStack.Push(segment);
        }

        public void Dispose()
        {
            while (this.segmentStack.Count > 0)
            {
                KcpSegment segment = this.segmentStack.Pop();
                KcpBuffer buffer = segment.data;
                //将buffer放回bufferAllotter，统一管理
                this.bufferAllotter.Put(buffer);
                segment.Dispose();
            }
            this.segmentStack.Clear();
            
            this.bufferAllotter.Dispose();
        }
    }
}
