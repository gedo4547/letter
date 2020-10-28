using System;
using System.Buffers;
using System.Buffers.Binary;
using System.ComponentModel;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Letter
{
    public struct WrappedStreamWriter
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public WrappedStreamWriter(PipeWriter pipeWriter, ref BinaryOrder order)
        {
            this.order = order;
            this.pipeWriter = pipeWriter;
            this.operators = BinaryOrderOperatorsFactory.GetOperators(order);
        }

        private readonly BinaryOrder order;
        private readonly PipeWriter pipeWriter;
        private readonly IBinaryOrderOperators operators;
        
        public BinaryOrder Order
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.order;
        }

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
            this.pipeWriter.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_2];
            this.operators.WriteInt16(span, ref value);
            this.pipeWriter.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ushort value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_2];
            this.operators.WriteUInt16(span, ref value);
            this.pipeWriter.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_4];
            this.operators.WriteInt32(span, ref value);
            this.pipeWriter.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_4];
            this.operators.WriteUInt32(span, ref value);
            this.pipeWriter.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_8];
            this.operators.WriteInt64(span, ref value);
            this.pipeWriter.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong value)
        {
            Span<byte> span = stackalloc byte[ByteSizeConstants.Size_8];
            this.operators.WriteUInt64(span, ref value);
            this.pipeWriter.Write(span);
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
            
            this.pipeWriter.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref Span<byte> span)
        {
            if (span.Length < 1)
            {
                throw new ArgumentNullException(nameof(span));  
            }
            
            this.pipeWriter.Write(span);
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
                this.pipeWriter.Write(sequence.First.Span);
            }
            else
            {
                foreach (var memory in sequence)
                {
                    this.pipeWriter.Write(memory.Span);
                }
            }
        }

        //To keep the API consistent
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            
        }
    }
}