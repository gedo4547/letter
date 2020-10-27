using System;

namespace Letter
{
    public interface IWrappedWriter
    {
        void Write(in ReadOnlySpan<byte> span);
    }
}