using System.Buffers;
using System.Buffers.Binary;

namespace System.IO.Pipelines
{
    public class DgramPipeline_1
    {
        private const int INIT_SEGMENT_POOL_SIZE = 8;
        private const int MAX_SEGMENT_POOL_SIZE = 256;

        public DgramPipeline_1(BinaryOrder order, MemoryPool<byte> memoryPool)
        {
            this.memoryPool = memoryPool;
            this.memoryBlockSize = this.memoryPool.MaxBufferSize;
            this.operators = BinaryOrderOperatorsFactory.GetOperators(order);
            this.entityBufferSegmentStack = new BufferStack<MemorySegment>(INIT_SEGMENT_POOL_SIZE);
        }
        
        
        private MemoryPool<byte> memoryPool;
        private IBinaryOrderOperators operators;
        private BufferStack<MemorySegment> entityBufferSegmentStack;
        
        private int memoryBlockSize;
        private MemorySegment headBufferSegment = null;
        private MemorySegment tailBufferSegment = null;
        
        private object sync = new object();

        public MemorySegment GetSegment()
        {
            lock (this.sync)
            {
                MemorySegment segment;
                if (!this.entityBufferSegmentStack.TryPop(out segment))
                {
                    segment = new MemorySegment(this.entityBufferSegmentStack);
                    segment.SetMemoryBlock(this.memoryPool.Rent());
                }

                this.AddNextBuffer(segment);

                return segment;
            }
        }
        
        private void AddNextBuffer(MemorySegment segment)
        {
            if (this.headBufferSegment == null && this.tailBufferSegment == null)
            {
                this.headBufferSegment = segment;
                this.tailBufferSegment = segment;
            }
            else
            {
                this.tailBufferSegment.SetNext(segment);
                this.tailBufferSegment = segment;
            }
        }
    }
}