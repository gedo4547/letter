using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    public interface IWrappedWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Memory<byte> GetWritableMemory(int length);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Span<byte> GetWritableSpan(int length);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriterAdvance(int length);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Write(in ReadOnlySpan<byte> span);
    }
}