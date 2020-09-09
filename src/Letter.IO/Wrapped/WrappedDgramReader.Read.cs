﻿using System;
using System.Runtime.CompilerServices;

namespace Letter.IO
{
    public ref partial struct WrappedDgramReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadInt8()
        {
            return (sbyte) this.ReadUInt8();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadUInt8()
        {
            var temp = this.ReadRange(ByteSizeConstants.Size_1);
            var arr = temp.GetBinaryArray();
            return arr.Array[arr.Offset];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
        {
            var temp = this.ReadRange(ByteSizeConstants.Size_2);
            return this.convertor.ReadInt16(temp.Span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
            var temp = this.ReadRange(ByteSizeConstants.Size_2);
            return this.convertor.ReadUInt16(temp.Span);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            var temp = this.ReadRange(ByteSizeConstants.Size_4);
            return this.convertor.ReadInt32(temp.Span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
        {
            var temp = this.ReadRange(ByteSizeConstants.Size_4);
            return this.convertor.ReadUInt32(temp.Span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
        {
            var temp = this.ReadRange(ByteSizeConstants.Size_8);
            return this.convertor.ReadInt64(temp.Span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
        {
            var temp = this.ReadRange(ByteSizeConstants.Size_8);
            return this.convertor.ReadUInt64(temp.Span);
        }

        public ArraySegment<byte> ReadBytes()
        {
            return this.ReadBytes(this.Length - this.readIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> ReadBytes(int length)
        {
            return this.ReadRange(length).GetBinaryArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> ReadRange(int length)
        {
            var tempMemory = this.memory.Slice(this.readIndex, length);
            this.readIndex += length;
            return tempMemory;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> ToArray()
        {
            return this.memory.GetBinaryArray();
        }
    }
}