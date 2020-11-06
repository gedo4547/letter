namespace System.Buffers
{
    public sealed class MemoryPoolOptions
    {
        public int MemoryBlockSize { get; set; }
        public int MemoryBlockCount { get; set; }

        public override string ToString()
        {
            return $"BlockSize:{MemoryBlockSize}, BlockCount:{MemoryBlockCount}";
        }
    }
}