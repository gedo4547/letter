using System;
using System.Buffers;
using System.Collections.Generic;

namespace System.Net
{
    sealed class MemoryBlockAllotter : IAllotter<MemoryBlock>
    {
        public MemoryBlockAllotter(MemoryPool<byte> memoryPool)
        {
            this.memoryPool = memoryPool;
        }

        private MemoryPool<byte> memoryPool;
        
        private Stack<MemoryBlock> memoryBlockStack = new Stack<MemoryBlock>();

        public MemoryBlock Get()
        {
            if(this.memoryBlockStack.Count > 0)
            {
                return this.memoryBlockStack.Pop();
            }
            
            var memoryBlock = new MemoryBlock();
            memoryBlock.SetMemoryBlock(memoryPool.Rent());

            return memoryBlock;
        }

        public void Put(MemoryBlock item)
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
