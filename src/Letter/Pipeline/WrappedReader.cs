using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Letter.Box
{
    internal delegate void ReaderFlushDelegate(SequencePosition startPos, SequencePosition endPos);

    public ref struct WrappedReader
    {
        internal WrappedReader(ReadOnlySequence<byte> buffer, BinaryOrder order, ReaderFlushDelegate readerFlush)
        {
            this.buffer = buffer;
            this.readerFlush = readerFlush;
            this.operators = BinaryOrderOperatorsFactory.GetOperators(order);
        }

        private ReadOnlySequence<byte> buffer;
        private readonly ReaderFlushDelegate readerFlush;
        private readonly IBinaryOrderOperators operators;

        public long Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.buffer.Length; }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsLengthEnough(long length)
        {
            return this.Length >= length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadInt8()
        {
            return (sbyte) this.ReadUInt8();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadUInt8()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_1);
            var arr = segment.First.GetBinaryArray();
            return arr.Array[arr.Offset];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_2);
            
            if (segment.First.Length >= ByteSizeConstants.Size_2)
                return this.operators.ReadInt16(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_2];
                segment.CopyTo(local);
                return this.operators.ReadInt16(local);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_2);
            
            if (segment.First.Length >= ByteSizeConstants.Size_2)
                return this.operators.ReadUInt16(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_2];
                segment.CopyTo(local);
                return this.operators.ReadUInt16(local);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_4);
            
            
            if (segment.First.Length >= ByteSizeConstants.Size_4)
                return this.operators.ReadInt32(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_4];
                segment.CopyTo(local);
                return this.operators.ReadInt32(local);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_4);
            
            if (segment.First.Length >= ByteSizeConstants.Size_4)
                return this.operators.ReadUInt32(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_4];
                segment.CopyTo(local);
                return this.operators.ReadUInt32(local);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long ReadInt64()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_8);
            
            if (segment.First.Length >= ByteSizeConstants.Size_8)
                return this.operators.ReadInt64(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_8];
                segment.CopyTo(local);
                return this.operators.ReadInt64(local);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_8);
            
            if (segment.First.Length >= ByteSizeConstants.Size_8)
                return this.operators.ReadUInt64(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_8];
                segment.CopyTo(local);
                return this.operators.ReadUInt64(local);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySequence<byte> ReadRange(SequencePosition endPosition)
        {
            var segment = this.buffer.Slice(buffer.Start, endPosition);
            this.buffer = this.buffer.Slice(endPosition);
            return segment;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySequence<byte> ReadRange(int length)
        {
            var endPos = this.buffer.GetPosition(length, this.buffer.Start);
            var segment = this.buffer.Slice(buffer.Start, endPos);
            this.buffer = this.buffer.Slice(endPos);
            return segment;
        }


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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            this.readerFlush(this.buffer.Start, this.buffer.End);
        }
    }
}