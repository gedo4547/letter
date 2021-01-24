using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;

namespace KcpProject
{
    sealed class KcpMemory : ASegment
    {
        public KcpMemory(Stack<KcpMemory> stack)
        {
            this.stack = stack;
        }

        private Stack<KcpMemory> stack;
        private IMemoryOwner<byte> memoryOwner;

        public void SetMemoryBlock(IMemoryOwner<byte> memoryOwner)
        {
            this.memoryOwner = memoryOwner;
            base.SetAvailableMemory(this.memoryOwner.Memory);
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}