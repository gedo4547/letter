using System;
using System.Buffers;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Letter
{
    public struct WrappedDgramWriter
    {
        public delegate void MemoryWritePushDelegate(UdpMessageNode node);
        
        public WrappedDgramWriter(UdpMessageNode node, ref BinaryOrder order, MemoryWritePushDelegate onMemoryPush)
        {
            this.order = order;
            this.node = node;
            this.onMemoryPush = onMemoryPush;
            this.writeLength = 0;
            this.operators = BinaryOrderOperatorsFactory.GetOperators(order);
        }

        private BinaryOrder order;
        private UdpMessageNode node;
        private MemoryWritePushDelegate onMemoryPush;
        private readonly IBinaryOrderOperators operators; 

        
        private int writeLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(sbyte value)
        {
            this.Write((byte) value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte value)
        {
            Span<byte> local = stackalloc byte[ByteSizeConstants.Size_1];
            MemoryMarshal.Write<byte>(local, ref value);
            InternalWriteSpan(local, ref this.writeLength, this.node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short value)
        {
            Span<byte> local = stackalloc byte[ByteSizeConstants.Size_2];
            this.operators.WriteInt16(local, ref value);
            InternalWriteSpan(local, ref this.writeLength, this.node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ushort value)
        {
            Span<byte> local = stackalloc byte[ByteSizeConstants.Size_2];
            this.operators.WriteUInt16(local, ref value);
            InternalWriteSpan(local, ref this.writeLength, this.node);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value)
        {
            Span<byte> local = stackalloc byte[ByteSizeConstants.Size_4];
            this.operators.WriteInt32(local, ref value);
            InternalWriteSpan(local, ref this.writeLength, this.node);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint value)
        {
            Span<byte> local = stackalloc byte[ByteSizeConstants.Size_4];
            this.operators.WriteUInt32(local, ref value);
            InternalWriteSpan(local, ref this.writeLength, this.node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long value)
        {
            Span<byte> local = stackalloc byte[ByteSizeConstants.Size_8];
            this.operators.WriteInt64(local, ref value);
            InternalWriteSpan(local, ref this.writeLength, this.node);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong value)
        {
            Span<byte> local = stackalloc byte[ByteSizeConstants.Size_8];
            this.operators.WriteUInt64(local, ref value);
            InternalWriteSpan(local, ref this.writeLength, this.node);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] bytes)
        {
            this.Write(bytes, 0, bytes.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] bytes, int offset, int count)
        {
            var span = new ReadOnlySpan<byte>(bytes, offset, count);
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
        public void Write(ref Span<byte> span)
        {
            var tempSpan = (ReadOnlySpan<byte>) span;
            Write(ref tempSpan);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref ReadOnlySpan<byte> span)
        {
            if (span.Length < 1)
            {
                throw new ArgumentNullException(nameof(span));
            }
            
            InternalWriteSpan(span, ref writeLength, this.node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref ReadOnlySequence<byte> sequence)
        {
            if (sequence.IsEmpty)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            if ( sequence.IsSingleSegment)
            {
                InternalWriteSpan(sequence.First.Span, ref writeLength, this.node);
            }
            else
            {
                foreach (var memory in sequence)
                {
                    InternalWriteSpan(memory.Span, ref writeLength, this.node);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InternalWriteSpan(in ReadOnlySpan<byte> span, ref int writeLength, UdpMessageNode node)
        {
            node.Write(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            this.onMemoryPush(this.node);
        }
    }
}