using System;
using System.IO.Pipelines;
using System.Collections.Generic;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Letter.Kcp.lib__
{
    sealed class KcpMemoryBlock : ASegment
    {
        public KcpMemoryBlock(Stack<KcpMemoryBlock> memoryBlockStack)
        {
            this.memoryBlockStack = memoryBlockStack;
        }

        private Stack<KcpMemoryBlock> memoryBlockStack;

        private IMemoryOwner<byte> memoryOwner;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMemoryBlock(IMemoryOwner<byte> memoryOwner)
        {
            this.memoryOwner = memoryOwner;
            base.SetAvailableMemory(this.memoryOwner.Memory);
        }

        public override void Reset()
        {
            this.memoryBlockStack.Push(this);

            base.Reset();
        }

        public override void Dispose()
        {
            this.memoryOwner.Dispose();
            

            base.Dispose();
        }
    }
}
