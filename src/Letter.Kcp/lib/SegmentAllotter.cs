using System;
using System.Buffers;
using System.Collections.Generic;

namespace System.Net
{
    class SegmentAllotter : IAllotter<Segment>
    {
        public SegmentAllotter(MemoryPool<byte> memoryPool, bool littleEndian)
        {
            this.littleEndian = littleEndian;
            this.bufferAllotter = new BufferAllotter(memoryPool);
        }

        private bool littleEndian;
        private BufferAllotter bufferAllotter;

        private Stack<Segment> segmentStack = new Stack<Segment>();

        public Segment Get()
        {
            if (this.segmentStack.Count > 0)
            {
                return this.segmentStack.Pop();
            }
            
            Buffer buffer = this.bufferAllotter.Get();
            return new Segment(this.littleEndian, buffer);
        }

        public void Put(Segment segment)
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
                Segment segment = this.segmentStack.Pop();
                Buffer buffer = segment.data;
                //将buffer放回bufferAllotter，统一管理
                this.bufferAllotter.Put(buffer);
                segment.Dispose();
            }
            this.segmentStack.Clear();
            
            this.bufferAllotter.Dispose();
        }
    }
}
