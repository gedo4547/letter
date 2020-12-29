using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace Letter.Kcp
{
    sealed class WrappedMemory : IWrappedWriter
    {
        public WrappedMemory()
        {
        }

        public WrappedMemory(IMemoryOwner<byte> memoryOwner)
        {
            this.memoryOwner = memoryOwner;
        }

        private int writedLength = 0;
        private IMemoryOwner<byte> memoryOwner;

        public void SettingMemory(IMemoryOwner<byte> memoryOwner, int writedLength)
        {
            this.writedLength = writedLength;
            this.memoryOwner = memoryOwner;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> GetWritableMemory(int length)
        {
            if (this.memoryOwner.Memory.Length < length)
                throw new ArgumentOutOfRangeException();
            return this.memoryOwner.Memory.Slice(0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetWritableSpan(int length)
        {
            return this.GetWritableMemory(length).Span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriterAdvance(int length)
        {
            this.writedLength += length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in ReadOnlySpan<byte> span)
        {
            int length = span.Length;
            var writableSpan = this.GetWritableSpan(length);
            span.CopyTo(writableSpan);
            this.WriterAdvance(length);
        }

        public Memory<byte> GetReadableMemory()
        {
            int length = this.writedLength;
            this.writedLength = 0;
            return this.memoryOwner.Memory.Slice(0, length);
        }
    }
}