using System.Buffers;
using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    public abstract class ASegment : ReadOnlySequenceSegment<byte>, IDisposable
    {
        public object Token
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.length; }
        }

        public int ReadableLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.writedLength - this.readedLength; }
        }

        public int WritableLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.length - this.writedLength; }
        }

        public ASegment ChildSegment
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.childSegment; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set { this.Next = value; this.childSegment = value; }
        }

        private int length;
        private int writedLength;
        private int readedLength;
        private Memory<byte> availableMemory;
        
        private ASegment childSegment;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SetAvailableMemory(Memory<byte> memory)
        {
            this.availableMemory = memory;
            this.length = this.availableMemory.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> GetWritableMemory(int length)
        {
            return this.availableMemory.Slice(this.writedLength, length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<byte> GetReadableMemory()
        {
            return base.Memory;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReaderAdvance(int length)
        {
            this.readedLength += length;
            this.SettingMemorySegment();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriterAdvance(int length)
        {
            this.writedLength += length;
            this.SettingMemorySegment();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SettingMemorySegment()
        {
            int start = this.readedLength;
            int length = this.writedLength - this.readedLength;
            base.Memory = this.availableMemory.Slice(start, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetNext(ASegment segment)
        {
            this.ChildSegment = segment;

            segment = this;

            while (segment.Next != null)
            {
                segment.ChildSegment.RunningIndex = segment.RunningIndex + segment.writedLength;
                
                segment = segment.ChildSegment;
            }
        }


        public virtual void Reset()
        {
            this.writedLength = 0;
            this.readedLength = 0;
            base.RunningIndex = 0;
            this.ChildSegment = null;
        }

        public virtual void Dispose()
        {
            this.length = 0;
            this.writedLength = 0;
            this.readedLength = 0;
            this.availableMemory = default;
            
            base.Memory = default;
            base.RunningIndex = 0;
            
            this.ChildSegment = null;
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long GetLength(ASegment startSegment, int startIndex, ASegment endSegment, int endIndex)
        {
            return (endSegment.RunningIndex + (uint)endIndex) - (startSegment.RunningIndex + (uint)startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long GetLength(long startPosition, ASegment endSegment, int endIndex)
        {
            return (endSegment.RunningIndex + (uint)endIndex) - startPosition;
        }
    }
}