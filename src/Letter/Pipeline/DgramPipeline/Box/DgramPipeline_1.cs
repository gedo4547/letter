using System.Buffers;
using System.Buffers.Binary;

namespace System.IO.Pipelines
{
    public class DgramPipeline_1
    {
        private const int INIT_SEGMENT_POOL_SIZE = 8;
        //private const int MAX_SEGMENT_POOL_SIZE = 256;

        public DgramPipeline_1(BinaryOrder order, MemoryPool<byte> memoryPool, Action rcvCallback)
        {
            this.memoryPool = memoryPool;
            this.memoryBlockSize = this.memoryPool.MaxBufferSize;
            this.operators = BinaryOrderOperatorsFactory.GetOperators(order);
            this.entityBufferSegmentStack = new BufferStack<MemorySegment>(INIT_SEGMENT_POOL_SIZE);
            
            this.rcvCallback = rcvCallback;
            this.Reader = new DgramPipeline_1Reader(this);
            this.Writer = new DgramPipeline_1Writer(this);
        }

        public DgramPipeline_1Reader Reader { get; private set; }
        public DgramPipeline_1Writer Writer { get; private set; }

        private Action rcvCallback;
        
        private MemoryPool<byte> memoryPool;
        private IBinaryOrderOperators operators;
        private BufferStack<MemorySegment> entityBufferSegmentStack;
        
        private int memoryBlockSize;
        private ASegment headBufferSegment = null;
        private ASegment tailBufferSegment = null;
        
        private object sync = new object();
        
        private int awaitRcv = 0;

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

        public void Advance()
        {
            lock (this.sync)
            {
                if (this.headBufferSegment == this.tailBufferSegment)
                {
                    this.headBufferSegment = null;
                    this.tailBufferSegment = null;
                }
                else
                {
                    this.tailBufferSegment = this.tailBufferSegment.ChildSegment;
                }
            }
        }
        
        public ReadDgramResult Read()
        {
            throw new NotImplementedException();
        }
        
        public void ReceiveAsync()
        {
            
        }

        public void FlushAsync()
        {
            
        }

        
    }
}