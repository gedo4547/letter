using System;
using System.Buffers;
using System.Collections.Generic;

namespace System.Net
{
    sealed class BufferAllotter : IAllotter<Buffer>
    {
        public BufferAllotter(MemoryPool<byte> memoryPool)
        {
            this.memoryAllotter = new MemoryBlockAllotter(memoryPool);
        }

        private MemoryBlockAllotter memoryAllotter;
        private Stack<Buffer> bufferStack = new Stack<Buffer>();

        public Buffer Get()
        {
            if(this.bufferStack.Count > 0)
            {
                return this.bufferStack.Pop();
            }

            return new Buffer(this.memoryAllotter);
        }

        public void Put(Buffer item)
        {
            if(item == null)
            {
                return;
            }

            this.bufferStack.Push(item);
        }

        public void Dispose()
        {
            foreach (Buffer buffer in this.bufferStack)
            {
                buffer.Dispose();
            }
            
            this.bufferStack.Clear();
            this.memoryAllotter.Dispose();
        }
    }
}
