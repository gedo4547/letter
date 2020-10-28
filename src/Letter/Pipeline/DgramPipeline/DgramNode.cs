using System;
using System.Buffers;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.IO.Pipelines
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DgramNode : IDgramNode, IWrappedWriter, IDisposable
    {
        public DgramNode(IMemoryOwner<byte> memoryOwner, Action<DgramNode> onRelease)
        {
            this.memoryOwner = memoryOwner;
            this.writeLength = 0;
            this.onRelease = onRelease;
        }

        private EndPoint point;
        private IMemoryOwner<byte> memoryOwner;
        private Action<DgramNode> onRelease;
        private int writeLength;
        
        public DgramNode next;
        
        public int Length
        {
            get
            {
                return this.memoryOwner.Memory.Length;
            }
        }

        public EndPoint Point
        {
            get { return point; }
        }

        public int ReadableLength
        {
            get { return this.writeLength; }
        }

        public Memory<byte> GetMomory()
        {
            return this.memoryOwner.Memory;
        }

        public Memory<byte> GetReadableBuffer()
        {
            return this.memoryOwner.Memory.Slice(0, this.writeLength);
        }

        public void SettingWriteLength(int length)
        {
            this.writeLength =+ length;
        }

        public void SettingPoint(EndPoint point)
        {
            this.point = point;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref ReadOnlyMemory<byte> memory)
        {
            var span = memory.Span;
            this.Write(span);
        }
        
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in ReadOnlySpan<byte> span)
        {
            int momoryLength = span.Length;
            var memory = this.GetWritableMemory(momoryLength);
            span.CopyTo(memory.Span);
            this.writeLength += momoryLength;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetWritableSpan(int length)
        {
            return this.GetWritableMemory(length).Span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> GetWritableMemory(int length)
        {
            return this.memoryOwner.Memory.Slice(this.writeLength, length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriterAdvance(int length)
        {
            this.writeLength += length;
        }



        public ReadOnlyMemory<byte> Read()
        {
            var memory = this.memoryOwner.Memory.Slice(0, this.writeLength);
            this.writeLength = 0;
            return memory;
        }

        public Task ReleaseAsync()
        {
            this.point = null;
            this.writeLength = 0;
            this.next = null;

            this.onRelease(this);
            return Task.CompletedTask;
        }
        
        public void Dispose()
        {
            this.point = null;
            this.writeLength = 0;
            this.next = null;
            this.memoryOwner.Dispose();
            this.memoryOwner = null;
            this.onRelease = null;
        }

        
    }
}