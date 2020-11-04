using System.Buffers;
using System.Buffers.Binary;
using System.Threading;

namespace System.IO.Pipelines
{
    public class DgramPipeline_1
    {
        private const int FALSE = 0;
        private const int TRUE = 1;
        
        private const int INIT_SEGMENT_POOL_SIZE = 8;
        //private const int MAX_SEGMENT_POOL_SIZE = 256;

        public DgramPipeline_1(BinaryOrder order, MemoryPool<byte> memoryPool, PipeScheduler scheduler, Action rcvCallback)
        {
            this.scheduler = scheduler;
            this.memoryPool = memoryPool;
            this.memoryBlockSize = this.memoryPool.MaxBufferSize;
            this.operators = BinaryOrderOperatorsFactory.GetOperators(order);
            this.entityBufferSegmentStack = new BufferStack<MemorySegment>(INIT_SEGMENT_POOL_SIZE);

            this.rcvCallback = (o) => { rcvCallback(); };
            this.Reader = new DgramPipeline_1Reader(this);
            this.Writer = new DgramPipeline_1Writer(this);
        }

        public DgramPipeline_1Reader Reader { get; private set; }
        public DgramPipeline_1Writer Writer { get; private set; }
        
        private PipeScheduler scheduler;
        private MemoryPool<byte> memoryPool;
        private IBinaryOrderOperators operators;
        private BufferStack<MemorySegment> entityBufferSegmentStack;
        
        private Action<object> rcvCallback;
        
        private int memoryBlockSize;
        private ASegment headBufferSegment = null;
        private ASegment tailBufferSegment = null;
        
        private object sync = new object();
        
        private int awaitRcv = FALSE;

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
                
                return segment;
            }
        }
        
        public void WriterAdvance(ASegment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException(nameof(segment));
            }
            
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

        public void ReaderAdvance()
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
            return new ReadDgramResult(this.headBufferSegment, this.tailBufferSegment);
        }
        
        public void ReceiveAsync()
        {
            Interlocked.Exchange(ref this.awaitRcv, TRUE);
            if (this.headBufferSegment != null)
            {
                this.PipelineNotify();
            }
        }

        public void FlushAsync() => this.PipelineNotify();

        private void PipelineNotify()
        {
            if (Interlocked.CompareExchange(ref this.awaitRcv, FALSE, TRUE) == TRUE)
            {
                this.scheduler.Schedule(this.rcvCallback, null);
            }
        }
    }
}