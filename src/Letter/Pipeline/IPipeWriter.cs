using System;

namespace Letter
{
    public interface IPipeWriter
    {
        void Write(in ReadOnlySpan<byte> span);
    }
}