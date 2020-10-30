using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    public interface IWrappedWriter
    {
        Memory<byte> GetWritableMemory(int length);
        
        Span<byte> GetWritableSpan(int length);
        
        void WriterAdvance(int length);
        
        void Write(in ReadOnlySpan<byte> span);
    }
}