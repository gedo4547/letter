using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.IO.Pipelines
{
    public delegate void WriterFlushDelegate(IWrappedWriter writer);
    
    public ref struct WrappedWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            int size = 1;
            var span = this.writer.GetWritableSpan(size);
            MemoryMarshal.Write<byte>(span, ref value);
            this.writer.WriterAdvance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short value)
        {
            int size = 2;
            Span<byte> span = this.writer.GetWritableSpan(size);
            this.operators.WriteInt16(span, ref value);
            this.writer.WriterAdvance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ushort value)
        {
            int size = 2;
            Span<byte> span = this.writer.GetWritableSpan(size);
            this.operators.WriteUInt16(span, ref value);
            this.writer.WriterAdvance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value)
        {
            int size = 4;
            Span<byte> span = this.writer.GetWritableSpan(size);
            this.operators.WriteInt32(span, ref value);
            this.writer.WriterAdvance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint value)
        {
            int size = 4;
            Span<byte> span = this.writer.GetWritableSpan(size);
            this.operators.WriteUInt32(span, ref value);
            this.writer.WriterAdvance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long value)
        {
            int size = 8;
            Span<byte> span = this.writer.GetWritableSpan(size);
            this.operators.WriteInt64(span, ref value);
            this.writer.WriterAdvance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong value)
        {
            int size = 8;
            Span<byte> span = this.writer.GetWritableSpan(size);
            this.operators.WriteUInt64(span, ref value);
            this.writer.WriterAdvance(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] bytes)
        {
            this.Write(bytes, 0, bytes.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] bytes, int offset, int count)
        {
            this.Write(new Span<byte>(bytes, offset, count));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref ArraySegment<byte> segment)
        {
            this.Write(segment.AsSpan());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref Memory<byte> memory)
        {
            this.Write(memory.Span);
        }

        public void Write(ref ReadOnlyMemory<byte> memory)
        {
            this.Write(memory.Span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in ReadOnlySpan<byte> span)
        {
            if (span.Length < 1)
            {
                throw new ArgumentNullException(nameof(span));  
            }
            
            this.writer.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in Span<byte> span)
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