using System.IO.Pipelines;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Letter.Kcp.lib__
{
    sealed class KcpMemoryBlock : ASegment
    {
        public KcpMemoryBlock(KcpMemoryBlockAllotter allotter)
        {
            this.allotter = allotter;
        }

        private IMemoryOwner<byte> memoryOwner;
        private KcpMemoryBlockAllotter allotter;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMemoryBlock(IMemoryOwner<byte> memoryOwner)
        {
            this.memoryOwner = memoryOwner;
            base.SetAvailableMemory(this.memoryOwner.Memory);
        }

        public override void Reset()
        {
            this.allotter.Put(this);

            base.Reset();
        }

        public override void Dispose()
        {
            this.memoryOwner.Dispose();

            base.Dispose();
        }
    }
}
