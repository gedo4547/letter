using System;
using System.Buffers;
using System.Collections.Generic;

namespace Letter.Kcp
{
    class WrappedMemoryPool : IDisposable
    {
        public WrappedMemoryPool(MemoryPool<byte> memoryPool, MemoryFlag flag)
        {
            this.flag = flag;
            this.memoryPool = memoryPool;
        }

        private MemoryFlag flag;
        private MemoryPool<byte> memoryPool;
        private Stack<WrappedMemory> stack = new Stack<WrappedMemory>();

        public WrappedMemory Pop()
        {
            if(stack.Count > 0)
            {
                return this.stack.Pop();
            }

            return new WrappedMemory(this.memoryPool.Rent(), this.flag);
        }

        public void Push(WrappedMemory memory)
        {
            if(memory == null)
            {
                return;
            }

            memory.Clear();
            this.stack.Push(memory);
        }

        public void Dispose()
        {
            if(this.stack != null)
            {
                foreach (var item in this.stack)
                {
                    item.Dispose();
                }

                this.stack.Clear();
                this.stack = null;
            }
            
            if(this.memoryPool != null)
            {
                this.memoryPool = null;
            }
        }
    }
}
