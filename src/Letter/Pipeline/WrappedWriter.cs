using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Letter.Box
{
    public delegate void WriterFlushDelegate(IWrappedWriter writer);
    
    public ref struct WrappedWriter
    {
        public WrappedWriter(IWrappedWriter writer, BinaryOrder order, WriterFlushDelegate writerFlush)
        {
            this.writer = writer;
            this.writerFlush = writerFlush;
            this.operators = BinaryOrderOperatorsFactory.GetOperators(order);
        }

        private readonly IWrappedWriter writer;
        private readonly WriterFlushDelegate writerFlush;
        private readonly IBinaryOrderOperators operators; 
        
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(sbyte value)
        {
            this.Write((byte)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_1];
            MemoryMarshal.Write<byte>(span, ref value);
            this.writer.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_2];
            this.operators.WriteInt16(span, ref value);
            this.writer.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ushort value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_2];
            this.operators.WriteUInt16(span, ref value);
            this.writer.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_4];
            this.operators.WriteInt32(span, ref value);
            this.writer.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_4];
            this.operators.WriteUInt32(span, ref value);
            this.writer.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_8];
            this.operators.WriteInt64(span, ref value);
            this.writer.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_8];
            this.operators.WriteUInt64(span, ref value);
            this.writer.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] bytes)
        {
            this.Write(bytes, 0, bytes.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] bytes, int offset, int count)
        {
            Span<byte> span = new Span<byte>(bytes, offset, count);
            this.Write(ref span);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref ArraySegment<byte> segment)
        {
            var span = segment.AsSpan();
            this.Write(ref span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref Memory<byte> memory)
        {
            var span = memory.Span;
            this.Write(ref span);
        }

        public void Write(ref ReadOnlyMemory<byte> memory)
        {
            var span = memory.Span;
            this.Write(ref span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref ReadOnlySpan<byte> span)
        {
            if (span.Length < 1)
            {
                throw new ArgumentNullException(nameof(span));  
            }
            
            this.writer.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref Span<byte> span)
        {
            if (span.Length < 1)
            {
                throw new ArgumentNullException(nameof(span));  
            }
            
            this.writer.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref ReadOnlySequence<byte> sequence)
        {
            if (sequence.IsEmpty)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            if (sequence.IsSingleSegment)
            {
                this.writer.Write(sequence.First.Span);
            }
            else
            {
                foreach (var memory in sequence)
                {
                    this.writer.Write(memory.Span);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            this.writerFlush(this.writer);
        }
    }
}