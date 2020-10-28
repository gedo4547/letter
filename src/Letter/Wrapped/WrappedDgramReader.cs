using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Letter
{
    //
    public partial struct WrappedDgramReader
    {
        public WrappedDgramReader(ref Memory<byte> memory, ref BinaryOrder order)
        {
            this.order = order;
            this.memory = memory;
            
            this.readIndex = 0;
            this.operators = BinaryOrderOperatorsFactory.GetOperators(order);
        }

        private readonly BinaryOrder order;
        private readonly Memory<byte> memory;
        private readonly IBinaryOrderOperators operators; 
        
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