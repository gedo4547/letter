using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Letter.Kcp.lib__
{
    sealed class KcpBuffer : IDisposable
    {
        public KcpBuffer(KcpMemoryBlockAllotter memoryBlockAllotter)
        {
            this.memoryBlockAllotter = memoryBlockAllotter;

            var memory = this.memoryBlockAllotter.Get();

            this.head = memory;
            this.tail = memory;
        }

        private KcpMemoryBlockAllotter memoryBlockAllotter;

        private KcpMemoryBlock head = null;
        private KcpMemoryBlock tail = null;

        public int ReadableLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get{ return (int)this.ReadableBuffer.Length; }
        }

        public ReadOnlySequence<byte> ReadableBuffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ReadOnlySequence<byte>(head, 0, tail, tail.ReadableLength); }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(byte[] bytes, int offset, int count)
        {
            this.WriteBytes(new ReadOnlySpan<byte>(bytes, offset, count));
        }

        public void WriteBytes(in ReadOnlySequence<byte> sequence)
        {
            foreach (var item in sequence)
            {
                this.WriteBytes(item.Span);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(in ReadOnlySpan<byte> span)
        {
            int writableSrcLength = span.Length;
            ReadOnlySpan<byte> writableSrcSpan = span;

            while(true)
            {
                KcpMemoryBlock writableBlock = this.GetWritableMemoryBlock();
                int writableLength = writableBlock.WritableLength;
                var writableMemory = writableBlock.GetWritableMemory(writableLength);

                ReadOnlySpan<byte> writableTempSpan;
                if(writableLength >= writableSrcLength)
                {
                    writableTempSpan = writableSrcSpan;
                }
                else
                {
                    writableTempSpan = writableSrcSpan.Slice(0, writableLength);
                }

                writableTempSpan.CopyTo(writableMemory.Span);
                writableBlock.WriterAdvance(writableTempSpan.Length);
                writableSrcLength -= writableTempSpan.Length;

                if(writableBlock.WritableLength < 1)
                {
                    this.MemoryAdvance();
                }

                if(writableSrcLength == 0)
                {
                    break;
                }

                writableSrcSpan = writableSrcSpan.Slice(writableTempSpan.Length);
            }
        }

        private KcpMemoryBlock GetWritableMemoryBlock()
        {
            return this.tail;
        }

        private void MemoryAdvance()
        {
            var memoryBlock = this.memoryBlockAllotter.Get();
            this.tail.SetNext(memoryBlock);
            this.tail = memoryBlock;
        }

        public void Reset()
        {
            if (this.TryClear(this.head))
            {
                this.head.Reset();
                this.head.SetNext(null);
                this.tail = this.head;
            }
        }

        public void Dispose()
        {
            this.TryClear(this.head);
        }

        private bool TryClear(KcpMemoryBlock startBlock)
        {
            if (startBlock == null)
            {
                return false;
            }

            KcpMemoryBlock block = startBlock;
            while (true)
            {
                KcpMemoryBlock child = block.Next as KcpMemoryBlock;
                
                block.Reset();
                block.SetNext(null);
                
                this.memoryBlockAllotter.Put(block);
                block = child;
                
                if (block == null)
                {
                    return true;
                }
            }
        }
    }
}
