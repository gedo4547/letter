using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Concurrent;


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
        private ConcurrentStack<WrappedMemory> stack = new ConcurrentStack<WrappedMemory>();

        public WrappedMemory Pop()
        {
            if (this.stack.TryPop(out var item))
            {
                return item;
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
