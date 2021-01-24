using System.Buffers;

namespace KcpProject
{
    class KcpBufferAllocator
    {
        public KcpBufferAllocator(MemoryPool<byte> memoryPool)
        {
            this.memoryPool = memoryPool;
        }

        private MemoryPool<byte> memoryPool;

        public KcpBuffer Get(int size)
        {
            return default;
        }

        public void Put(KcpBuffer buffer)
        {
            
        }
    }
}