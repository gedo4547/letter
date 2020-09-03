using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public static class BufferExtensions
    {
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> GetBinaryArray(in this Memory<byte> memory)
        {
            return ((ReadOnlyMemory<byte>)memory).GetBinaryArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> GetBinaryArray(in this ReadOnlyMemory<byte> memory)
        {
            if (!MemoryMarshal.TryGetArray(memory, out var result))
            {
                throw new InvalidOperationException("Buffer backed by array was expected");
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<byte> ToReadOnlyMemory(this Memory<byte> memory)
        {
            var segment = memory.GetBinaryArray();
            return new ReadOnlyMemory<byte>(segment.Array, segment.Offset, segment.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<byte> ToMemory(this ReadOnlyMemory<byte> memory)
        {
            var segment = memory.GetBinaryArray();
            return new Memory<byte>(segment.Array, segment.Offset, segment.Count);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryPositionOf<T>(in this ReadOnlySequence<T> source, T value, out SequencePosition position)
            where T : IEquatable<T>
        {
            if (!source.IsSingleSegment)
                return TryPositionOfMultiSegment<T>(source, value, out position);
            int num = source.First.Span.IndexOf<T>(value);
            if (num != -1)
            {
                position = source.GetPosition((long) num);
                return true;
            }

            position = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryPositionOfMultiSegment<T>(in ReadOnlySequence<T> source, T value, out SequencePosition position)
            where T : IEquatable<T>
        {
            SequencePosition start = source.Start;
            SequencePosition origin = start;
            ReadOnlyMemory<T> memory;
            while (source.TryGet(ref start, out memory, true))
            {
                int num = memory.Span.IndexOf<T>(value);
                if (num != -1)
                {
                    position = source.GetPosition((long) num, origin);
                    return true;
                }
                if (start.GetObject() != null)
                    origin = start;
                else
                    break;
            }

            position = default;
            return false;
        }
    }
}