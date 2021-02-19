using System;
using System.Buffers;
using System.Collections.Generic;

namespace System.Net
{
    sealed class KcpMemoryBlockAllotter : IKcpAllotter<KcpMemoryBlock>
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
            
            var memoryBlock = new KcpMemoryBlock();
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
            while(this.memoryBlockStack.Count > 0)
            {
                var item = this.memoryBlockStack.Pop();
                item.Dispose();
            }
            
            this.memoryBlockStack.Clear();
        }
    }
}
