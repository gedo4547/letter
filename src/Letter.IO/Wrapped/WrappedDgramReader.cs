using System;
using System.Runtime.CompilerServices;

namespace Letter.IO
{
    public ref partial struct WrappedDgramReader
    {
        internal WrappedDgramReader(ref Memory<byte> memory, ref BinaryOrder order)
        {
            this.order = order;
            this.memory = memory;
            
            this.readIndex = 0;
            this.convertor = BinaryOrderConvertorFactory.GetConvertor(order);
        }

        private readonly BinaryOrder order;
        private readonly Memory<byte> memory;
        private readonly IBinaryOrderConvertor convertor; 
        
        private int readIndex;

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.memory.Length; }
        }

        //保证api统一
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            
        }
    }
}