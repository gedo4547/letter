using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace Letter.IO
{
    public partial struct WrappedStreamReader
    {
        public WrappedStreamReader(PipeReader pipeReader, ref ReadOnlySequence<byte> buffer, ref BinaryOrder order)
        {
            this.order = order;
            this.buffer = buffer;
            this.pipeReader = pipeReader;
            this.convertor = BinaryOrderConvertorFactory.GetConvertor(order);

        }

        private readonly BinaryOrder order;
        private readonly PipeReader pipeReader;
        private readonly IBinaryOrderConvertor convertor; 
        
        
        public ReadOnlySequence<byte> buffer;
        
        
        public BinaryOrder Order
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.order;
        }

        public long Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.buffer.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            this.pipeReader.AdvanceTo(this.buffer.Start, this.buffer.End);
        }
    }
}