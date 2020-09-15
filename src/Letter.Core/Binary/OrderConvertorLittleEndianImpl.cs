using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Letter
{
    sealed class OrderConvertorLittleEndianImpl : IBinaryOrderConvertor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadInt16LittleEndian(span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadUInt16LittleEndian(span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadInt32LittleEndian(span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadUInt32LittleEndian(span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadInt64LittleEndian(span);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64(in ReadOnlySpan<byte> span) => BinaryPrimitives.ReadUInt64LittleEndian(span);
        
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(in Span<byte> destination, ref short value) => BinaryPrimitives.WriteInt16LittleEndian(destination, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(in Span<byte> destination, ref ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(destination, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(in Span<byte> destination, ref int value) => BinaryPrimitives.WriteInt32LittleEndian(destination, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(in Span<byte> destination, ref uint value) => BinaryPrimitives.WriteUInt32LittleEndian(destination, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(in Span<byte> destination, ref long value) => BinaryPrimitives.WriteInt64LittleEndian(destination, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(in Span<byte> destination, ref ulong value) => BinaryPrimitives.WriteUInt64LittleEndian(destination, value);
    }
}