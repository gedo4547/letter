using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Buffers.Binary;

namespace Letter.Kcp.lib__
{
    public static class KcpHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt8(in ReadOnlySequence<byte> sequence, byte value)
        {
            var span = sequence.First.ToMemory().Span;
            MemoryMarshal.Write<byte>(span, ref value);

            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt16_LE(in ReadOnlySequence<byte> sequence, ushort value)
        {
            const int length = 2;

            if(sequence.First.Length >= length)
            {
                Span<byte> writableSpan = sequence.First.ToMemory().Span;
                BinaryPrimitives.WriteUInt16LittleEndian(writableSpan, value);
            }
            else
            {
                Span<byte> local = stackalloc byte[length];
                BinaryPrimitives.WriteUInt16LittleEndian(local, value);
                for (int i = 0; i < length; i++)
                {
                    WriteUInt8(sequence.Slice(i), local[i]);
                }
            }

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt16_BE(in ReadOnlySequence<byte> sequence, ushort value)
        {
            const int length = 2;

            if(sequence.First.Length >= length)
            {
                Span<byte> writableSpan = sequence.First.ToMemory().Span;
                BinaryPrimitives.WriteUInt16BigEndian(writableSpan, value);
            }
            else
            {
                Span<byte> local = stackalloc byte[length];
                BinaryPrimitives.WriteUInt16BigEndian(local, value);
                for (int i = 0; i < length; i++)
                {
                    WriteUInt8(sequence.Slice(i), local[i]);
                }
            }

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt32_LE(in ReadOnlySequence<byte> sequence, uint value)
        {
            const int length = 4;

            if(sequence.First.Length >= length)
            {
                Span<byte> writableSpan = sequence.First.ToMemory().Span;
                BinaryPrimitives.WriteUInt32LittleEndian(writableSpan, value);
            }
            else
            {
                Span<byte> local = stackalloc byte[length];
                BinaryPrimitives.WriteUInt32LittleEndian(local, value);
                for (int i = 0; i < length; i++)
                {
                    WriteUInt8(sequence.Slice(i), local[i]);
                }
            }

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt32_BE(in ReadOnlySequence<byte> sequence, uint value)
        {
            const int length = 4;

            if(sequence.First.Length >= length)
            {
                Span<byte> writableSpan = sequence.First.ToMemory().Span;
                BinaryPrimitives.WriteUInt32BigEndian(writableSpan, value);
            }
            else
            {
                Span<byte> local = stackalloc byte[length];
                BinaryPrimitives.WriteUInt32BigEndian(local, value);
                for (int i = 0; i < length; i++)
                {
                    WriteUInt8(sequence.Slice(i), local[i]);
                }
            }

            return length;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt8(in ReadOnlySequence<byte> sequence, ref byte value)
        {
            value = sequence.First.ToMemory().Span[0];

            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt16_LE(in ReadOnlySequence<byte> sequence, ref ushort value)
        {
            const int length = 2;
            if(sequence.First.Length >= length)
            {
                value = BinaryPrimitives.ReadUInt16LittleEndian(sequence.First.Span);
            }
            else
            {
                Span<byte> local = stackalloc byte[length];
                sequence.Slice(0, length).CopyTo(local);
                value = BinaryPrimitives.ReadUInt16LittleEndian(local);
            }

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt16_BE(in ReadOnlySequence<byte> sequence, ref ushort value)
        {
            const int length = 2;
            if(sequence.First.Length >= length)
            {
                value = BinaryPrimitives.ReadUInt16BigEndian(sequence.First.Span);
            }
            else
            {
                Span<byte> local = stackalloc byte[length];
                sequence.Slice(0, length).CopyTo(local);
                value = BinaryPrimitives.ReadUInt16BigEndian(local);
            }

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt32_LE(in ReadOnlySequence<byte> sequence, ref uint value)
        {
            const int length = 4;
            if(sequence.First.Length >= length)
            {
                value = BinaryPrimitives.ReadUInt32LittleEndian(sequence.First.Span);
            }
            else
            {
                Span<byte> local = stackalloc byte[length];
                sequence.Slice(0, length).CopyTo(local);
                value = BinaryPrimitives.ReadUInt32LittleEndian(local);
            }

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt32_BE(in ReadOnlySequence<byte> sequence, ref uint value)
        {
            const int length = 4;
            if(sequence.First.Length >= length)
            {
                value = BinaryPrimitives.ReadUInt32BigEndian(sequence.First.Span);
            }
            else
            {
                Span<byte> local = stackalloc byte[length];
                sequence.Slice(0, length).CopyTo(local);
                value = BinaryPrimitives.ReadUInt32BigEndian(local);
            }

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
