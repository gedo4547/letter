using System;
using System.Buffers.Binary;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Letter.IO
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class BinarySequenceHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(in ReadOnlySpan<byte> span, BinaryOrder order)
        {
            short value = 0;
            switch (order)
            {
                case BinaryOrder.BigEndian: value = BinaryPrimitives.ReadInt16BigEndian(span); break;
                case BinaryOrder.LittleEndian: value = BinaryPrimitives.ReadInt16LittleEndian(span); break;
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(in ReadOnlySpan<byte> span, BinaryOrder order)
        {
            ushort value = 0;
            switch (order)
            {
                case BinaryOrder.BigEndian: value = BinaryPrimitives.ReadUInt16BigEndian(span); break;
                case BinaryOrder.LittleEndian: value = BinaryPrimitives.ReadUInt16LittleEndian(span); break;
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(in ReadOnlySpan<byte> span, BinaryOrder order)
        {
            int value = 0;
            switch (order)
            {
                case BinaryOrder.BigEndian: value = BinaryPrimitives.ReadInt32BigEndian(span); break;
                case BinaryOrder.LittleEndian: value = BinaryPrimitives.ReadInt32LittleEndian(span); break;
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(in ReadOnlySpan<byte> span, BinaryOrder order)
        {
            uint value = 0;
            switch (order)
            {
                case BinaryOrder.BigEndian: value = BinaryPrimitives.ReadUInt32BigEndian(span); break;
                case BinaryOrder.LittleEndian: value = BinaryPrimitives.ReadUInt32LittleEndian(span); break;
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(in ReadOnlySpan<byte> span, BinaryOrder order)
        {
            long value = 0;
            switch (order)
            {
                case BinaryOrder.BigEndian: value = BinaryPrimitives.ReadInt64BigEndian(span); break;
                case BinaryOrder.LittleEndian: value = BinaryPrimitives.ReadInt64LittleEndian(span); break;
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(in ReadOnlySpan<byte> span, BinaryOrder order)
        {
            ulong value = 0;
            switch (order)
            {
                case BinaryOrder.BigEndian: value = BinaryPrimitives.ReadUInt64BigEndian(span); break;
                case BinaryOrder.LittleEndian: value = BinaryPrimitives.ReadUInt64LittleEndian(span); break;
            }
            return value;
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16(in Span<byte> destination, ref short value, BinaryOrder order)
        {
            switch (order)
            {
                case BinaryOrder.BigEndian: BinaryPrimitives.WriteInt16BigEndian(destination, value); break;
                case BinaryOrder.LittleEndian: BinaryPrimitives.WriteInt16LittleEndian(destination, value); break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16(in Span<byte> destination, ref ushort value, BinaryOrder order)
        {
            switch (order)
            {
                case BinaryOrder.BigEndian: BinaryPrimitives.WriteUInt16BigEndian(destination, value); break;
                case BinaryOrder.LittleEndian: BinaryPrimitives.WriteUInt16LittleEndian(destination, value); break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32(in Span<byte> destination, ref int value, BinaryOrder order)
        {
            switch (order)
            {
                case BinaryOrder.BigEndian: BinaryPrimitives.WriteInt32BigEndian(destination, value); break;
                case BinaryOrder.LittleEndian: BinaryPrimitives.WriteInt32LittleEndian(destination, value); break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32(in Span<byte> destination, ref uint value, BinaryOrder order)
        {
            switch (order)
            {
                case BinaryOrder.BigEndian: BinaryPrimitives.WriteUInt32BigEndian(destination, value); break;
                case BinaryOrder.LittleEndian: BinaryPrimitives.WriteUInt32LittleEndian(destination, value); break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64(in Span<byte> destination, ref long value, BinaryOrder order)
        {
            switch (order)
            {
                case BinaryOrder.BigEndian: BinaryPrimitives.WriteInt64BigEndian(destination, value); break;
                case BinaryOrder.LittleEndian: BinaryPrimitives.WriteInt64LittleEndian(destination, value); break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64(in Span<byte> destination, ref ulong value, BinaryOrder order)
        {
            switch (order)
            {
                case BinaryOrder.BigEndian: BinaryPrimitives.WriteUInt64BigEndian(destination, value); break;
                case BinaryOrder.LittleEndian: BinaryPrimitives.WriteUInt64LittleEndian(destination, value); break;
            }
        }
    }
}
