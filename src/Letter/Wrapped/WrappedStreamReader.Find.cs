using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Letter
{
    public partial struct WrappedStreamReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindPosition(byte[] value, out SequencePosition lastPosition)
        {
            if (value == null || value.Length == 0)
            {
                throw new ArgumentNullException(nameof(value));
            }

            int valueLength = value.Length;
            if (valueLength == 1)
            {
                if (this.TryFindPosition(value[valueLength - 1], out lastPosition))
                {
                    lastPosition.Offet(1);
                }
            }

            SequencePosition startPosition = this.buffer.Start;
            while (true)
            {
                if (startPosition.GetInteger() >= this.buffer.End.GetInteger())
                {
                    break;
                }

                if (this.TryFindPosition(value[valueLength - 1], out var endPos))
                {
                    var tempEndPos = endPos.Offet(1);
                    var startPos = this.GetStartPosition(ref tempEndPos, valueLength);
                    var tempSequence = this.buffer.Slice(startPos, tempEndPos);
                    if (this.VerifySequence(ref tempSequence, value))
                    {
                        lastPosition = tempEndPos;
                        return true;
                    }

                    startPosition = tempEndPos;
                }
            }
            
            lastPosition = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindPosition(byte value, out SequencePosition position)
        {
            return TryFindPosition(value, this.buffer.Start, out position);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryFindPosition(byte value, in SequencePosition startPosition, out SequencePosition position)
        {
            return this.buffer.Slice(startPosition, this.buffer.End).TryPositionOf(value, out position);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SequencePosition GetStartPosition(ref SequencePosition endPos, int valueLength)
        {
            var endIndex = endPos.GetInteger();
            var startIndex = endIndex - valueLength;
            return new SequencePosition(endPos.GetObject(), startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool VerifySequence(ref ReadOnlySequence<byte> sequence, byte[] bytes)
        {
            if (sequence.First.Length < bytes.Length)
            {
                Span<byte> segment = stackalloc byte[bytes.Length];
                sequence.CopyTo(segment);
                return VerifyValueEquality(segment, bytes);
            }
            else
            {
                return VerifyValueEquality(sequence.First.GetBinaryArray(), bytes);
            }

            bool VerifyValueEquality(in Span<byte> sequenceSpan, byte[] value)
            {
                for (int i = 0; i < sequenceSpan.Length; i++)
                {
                    if (sequenceSpan[i] != value[i]) return false;
                }

                return true;
            }
        }
    }
}