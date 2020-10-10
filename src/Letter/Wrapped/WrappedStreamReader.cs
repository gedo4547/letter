using System.Buffers;
using System.ComponentModel;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace Letter
{
    public partial struct WrappedStreamReader
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public WrappedStreamReader(PipeReader pipeReader, ref ReadOnlySequence<byte> buffer, ref BinaryOrder order)
        {
            this.order = order;
            this.buffer = buffer;
            this.pipeReader = pipeReader;
            this.operators = BinaryOrderOperatorsFactory.GetOperators(order);

        }

        private readonly BinaryOrder order;
        private readonly PipeReader pipeReader;
        private readonly IBinaryOrderOperators operators; 
        
        
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            this.pipeReader.AdvanceTo(this.buffer.Start, this.buffer.End);
        }
    }
}