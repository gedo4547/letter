using System.Buffers;
using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    public sealed class MemorySegment : ASegment, IWrappedWriter
    {
        public MemorySegment(BufferStack<MemorySegment> stack)
        {
            this.stack = stack;
        }

        private IMemoryOwner<byte> memoryOwner;
        private BufferStack<MemorySegment> stack;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMemoryBlock(IMemoryOwner<byte> memoryOwner)
        {
            this.memoryOwner = memoryOwner;
            base.SetAvailableMemory(this.memoryOwner.Memory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetWritableSpan(int length)
        {
            return base.GetWritableMemory(length).Span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in ReadOnlySpan<byte> span)
        {
            int length = span.Length;
            var writableSpan = this.GetWritableSpan(length);
            span.CopyTo(writableSpan);
            base.WriterAdvance(length);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            this.memoryOwner.Dispose();
            this.memoryOwner = null;
            this.stack.Push(this);
        }
    }
}