// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace System.Buffers
{
    public static class SlabMemoryPoolFactory
    {
        public static MemoryPool<byte> shared = Create(new MemoryPoolOptions()
        {
            MemoryBlockSize = 4096,
            MemoryBlockCount = 32
        });

        public static MemoryPool<byte> Create(MemoryPoolOptions options)
        {
            options.MemoryBlockSize = BufferCapacityHelper.GetSuitableBufferSize(options.MemoryBlockSize);
            
#if DEBUG
            return new DiagnosticMemoryPool(CreateSlabMemoryPool(options.MemoryBlockSize, options.MemoryBlockCount));
#else
            return CreateSlabMemoryPool(options.MemoryBlockSize, options.MemoryBlockCount);
#endif
        }

        public static MemoryPool<byte> CreateSlabMemoryPool(int blockSize, int blockCount)
        {
            return new SlabMemoryPool(blockSize, blockCount);
        }
    }
}
