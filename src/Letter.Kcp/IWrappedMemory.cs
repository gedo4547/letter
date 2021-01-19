using System;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace Letter.Kcp
{
    public interface IWrappedMemory : IWrappedWriter
    {
        MemoryFlag Flag { get; }

        object Token { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Memory<byte> GetReadableMemory();
    }


   
}
