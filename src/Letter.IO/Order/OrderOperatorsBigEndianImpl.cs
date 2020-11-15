using System.Runtime.CompilerServices;

namespace System.Buffers.Binary
{
    sealed class OrderOperatorsBigEndianImpl : IBinaryOrderOperators
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadInt16BigEndian(span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadUInt16BigEndian(span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadInt32BigEndian(span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadUInt32BigEndian(span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadInt64BigEndian(span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadUInt64BigEndian(span);
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(in Span<byte> destination, short value) => BinaryPrimitives.WriteInt16BigEndian(destination, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(in Span<byte> destination, ushort value) => BinaryPrimitives.WriteUInt16BigEndian(destination, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(in Span<byte> destination, int value) => BinaryPrimitives.WriteInt32BigEndian(destination, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(in Span<byte> destination, uint value) => BinaryPrimitives.WriteUInt32BigEndian(destination, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(in Span<byte> destination, long value) => BinaryPrimitives.WriteInt64BigEndian(destination, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(in Span<byte> destination, ulong value) => BinaryPrimitives.WriteUInt64BigEndian(destination, value);
    }
}