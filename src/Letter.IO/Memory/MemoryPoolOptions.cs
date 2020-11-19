namespace System.Buffers
{
    public sealed class MemoryPoolOptions
    {
        public MemoryPoolOptions(int blockSize, int blockCount)
        {
            this.MemoryBlockSize = blockSize;
            this.MemoryBlockCount = blockCount;
        }

        public int MemoryBlockSize { get; }
        public int MemoryBlockCount { get; }

        public override string ToString()
        {
            return $"BlockSize:{MemoryBlockSize}, BlockCount:{MemoryBlockCount}";
        }
    }
}