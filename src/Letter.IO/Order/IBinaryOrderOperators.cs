using System.Runtime.CompilerServices;

namespace System.Buffers.Binary
{
    public interface IBinaryOrderOperators
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        short ReadInt16(in ReadOnlySpan<byte> span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ushort ReadUInt16(in ReadOnlySpan<byte> span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int ReadInt32(in ReadOnlySpan<byte> span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint ReadUInt32(in ReadOnlySpan<byte> span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long ReadInt64(in ReadOnlySpan<byte> span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ulong ReadUInt64(in ReadOnlySpan<byte> span);

        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteInt16(in Span<byte> destination, short value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteUInt16(in Span<byte> destination, ushort value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteInt32(in Span<byte> destination, int value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteUInt32(in Span<byte> destination, uint value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteInt64(in Span<byte> destination, long value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteUInt64(in Span<byte> destination, ulong value);
    }
}