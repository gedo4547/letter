using System;
using System.Buffers;
using System.Collections.Generic;

namespace System.Net.Sockets
{
    sealed class KcpBufferAllotter : IKcpAllotter<KcpBuffer>
    {
        public KcpBufferAllotter(MemoryPool<byte> memoryPool)
        {
            // this.memoryAllotter = new KcpMemoryBlockAllotter(memoryPool);
        }

        // private KcpMemoryBlockAllotter memoryAllotter;
        private Stack<KcpBuffer> bufferStack = new Stack<KcpBuffer>();

        public KcpBuffer Get()
        {
            if(this.bufferStack.Count > 0)
            {
                return this.bufferStack.Pop();
            }

            // return new KcpBuffer(this.memoryAllotter);
            return default;
        }

        public void Put(KcpBuffer item)
        {
            if(item == null)
            {
                return;
            }

            this.bufferStack.Push(item);
        }

        public void Dispose()
        {
            foreach (KcpBuffer buffer in this.bufferStack)
            {
                buffer.Dispose();
            }
            
            this.bufferStack.Clear();
            // this.memoryAllotter.Dispose();
        }
    }
}
