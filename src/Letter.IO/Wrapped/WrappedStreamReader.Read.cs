using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Letter.IO
{
    public partial struct WrappedStreamReader
    {
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
                return this.convertor.ReadInt16(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_2];
                segment.CopyTo(local);
                return this.convertor.ReadInt16(local);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_2);
            
            if (segment.First.Length >= ByteSizeConstants.Size_2)
                return this.convertor.ReadUInt16(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_2];
                segment.CopyTo(local);
                return this.convertor.ReadUInt16(local);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_4);
            
            
            if (segment.First.Length >= ByteSizeConstants.Size_4)
                return this.convertor.ReadInt32(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_4];
                segment.CopyTo(local);
                return this.convertor.ReadInt32(local);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_4);
            
            if (segment.First.Length >= ByteSizeConstants.Size_4)
                return this.convertor.ReadUInt32(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_4];
                segment.CopyTo(local);
                return this.convertor.ReadUInt32(local);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long ReadInt64()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_8);
            
            if (segment.First.Length >= ByteSizeConstants.Size_8)
                return this.convertor.ReadInt64(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_8];
                segment.CopyTo(local);
                return this.convertor.ReadInt64(local);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
        {
            var segment = this.ReadRange(ByteSizeConstants.Size_8);
            
            if (segment.First.Length >= ByteSizeConstants.Size_8)
                return this.convertor.ReadUInt64(segment.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[ByteSizeConstants.Size_8];
                segment.CopyTo(local);
                return this.convertor.ReadUInt64(local);
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
    }
}