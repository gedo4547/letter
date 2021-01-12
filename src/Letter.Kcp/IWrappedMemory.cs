using System;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace Letter.Kcp
{
    public interface IWrappedMemory : IWrappedWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Memory<byte> GetReadableMemory();
    }
}
