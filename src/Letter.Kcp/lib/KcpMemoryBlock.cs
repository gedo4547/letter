using System.IO.Pipelines;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace System.Net
{
    sealed class KcpMemoryBlock : ASegment
    {
        private IMemoryOwner<byte> memoryOwner;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            this.memoryOwner.Dispose();

            base.Dispose();
        }
    }
}
