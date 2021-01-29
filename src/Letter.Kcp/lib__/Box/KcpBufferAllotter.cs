using System;
using System.Buffers;
using System.Collections.Generic;

namespace Letter.Kcp.lib__
{
    sealed class KcpBufferAllotter : IAllotter<KcpBuffer>
    {
        public KcpBufferAllotter(MemoryPool<byte> memoryPool)
        {
            this.memoryAllotter = new KcpMemoryBlockAllotter(memoryPool);
        }

        private KcpMemoryBlockAllotter memoryAllotter;
        private Stack<KcpBuffer> bufferStack = new Stack<KcpBuffer>();

        public KcpBuffer Get()
        {
            if(this.bufferStack.Count > 0)
            {
                return this.bufferStack.Pop();
            }

            return new KcpBuffer(this.memoryAllotter);
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
            throw new NotImplementedException();
        }
    }
}
