using System;

namespace Letter
{
    public interface IWriter
    {
        void Write(in ReadOnlySpan<byte> span);
    }
}