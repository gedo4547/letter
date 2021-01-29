using System;
using System.Buffers;
using System.Collections.Generic;

namespace Letter.Kcp.lib__
{
    sealed class KcpMemoryBlockAllotter : IAllotter<KcpMemoryBlock>
    {
        public KcpMemoryBlockAllotter(MemoryPool<byte> memoryPool)
        {
            this.memoryPool = memoryPool;
        }

        private MemoryPool<byte> memoryPool;
        private Stack<KcpMemoryBlock> memoryBlockStack = new Stack<KcpMemoryBlock>();

        public KcpMemoryBlock Get()
        {
            if(this.memoryBlockStack.Count > 0)
            {
                return this.memoryBlockStack.Pop();
            }
            
            var memoryBlock = new KcpMemoryBlock(this.memoryBlockStack);
            memoryBlock.SetMemoryBlock(memoryPool.Rent());

            return memoryBlock;
        }

        public void Put(KcpMemoryBlock item)
        {
            if(item == null)
            {
                return;
            }

            this.memoryBlockStack.Push(item);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
