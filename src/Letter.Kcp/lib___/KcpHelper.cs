using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Letter.IO.Kcplib
{
    static class KcpHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt8(in Span<byte> span, byte value)
        {
            MemoryMarshal.Write<byte>(span, ref value);

            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt16_LE(in Span<byte> span, ushort value)
        {
            const int length = 2;

            BinaryPrimitives.WriteUInt16LittleEndian(span, value);

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt16_BE(in Span<byte> span, ushort value)
        {
            const int length = 2;

            BinaryPrimitives.WriteUInt16BigEndian(span, value);

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt32_LE(in Span<byte> span, uint value)
        {
            const int length = 4;

            BinaryPrimitives.WriteUInt32LittleEndian(span, value);

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt32_BE(in Span<byte> span, uint value)
        {
            const int length = 4;

            BinaryPrimitives.WriteUInt32BigEndian(span, value);

            return length;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt8(in Span<byte> span, ref byte value)
        {
            value = span[0];

            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt16_LE(in Span<byte> span, ref ushort value)
        {
            const int length = 2;

            value = BinaryPrimitives.ReadUInt16LittleEndian(span);

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt16_BE(in Span<byte> span, ref ushort value)
        {
            const int length = 2;

            value = BinaryPrimitives.ReadUInt16BigEndian(span);
           
            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt32_LE(in Span<byte> span, ref uint value)
        {
            const int length = 4;

            value = BinaryPrimitives.ReadUInt32LittleEndian(span);

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt32_BE(in Span<byte> span, ref uint value)
        {
            const int length = 4;
            
            value = BinaryPrimitives.ReadUInt32BigEndian(span);

            return length;
        }

        private static DateTime refTime = DateTime.Now;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 currentMS()
        {
            var ts = DateTime.Now.Subtract(refTime);
            return (UInt32)ts.TotalMilliseconds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 _imin_(UInt32 a, UInt32 b)
        {
            return a <= b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 _imax_(UInt32 a, UInt32 b)
        {
            return a >= b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 _ibound_(UInt32 lower, UInt32 middle, UInt32 upper)
        {
            return _imin_(_imax_(lower, middle), upper);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 _itimediff(UInt32 later, UInt32 earlier)
        {
            return ((Int32)(later - earlier));
        }
    }
}
