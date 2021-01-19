using System.Buffers;
using System.Collections.Concurrent;
using System.Threading;

namespace System.IO.Pipelines
{
    public sealed class DgramPipeline
    {
        private const int FALSE = 0;
        private const int TRUE = 1;

        public DgramPipeline(MemoryPool<byte> memoryPool, PipeScheduler scheduler, Action rcvCallback)
        {
            this.scheduler = scheduler;
            this.memoryPool = memoryPool;
            this.memoryBlockSize = this.memoryPool.MaxBufferSize;
            this.segmentStack = new ConcurrentStack<MemorySegment>();

            this.rcvCallback = (o) => { rcvCallback(); };
            this.Reader = new DgramPipelineReader(this);
            this.Writer = new DgramPipelineWriter(this);
        }

        public DgramPipelineReader Reader { get; private set; }
        public DgramPipelineWriter Writer { get; private set; }
        
        private PipeScheduler scheduler;
        private MemoryPool<byte> memoryPool;
        private ConcurrentStack<MemorySegment> segmentStack;


        private Action<object> rcvCallback;
        
        private int memoryBlockSize;
        private ASegment awaitWriteSegment = null;
        
        private ASegment headBufferSegment = null;
        private ASegment tailBufferSegment = null;
        
        private object sync = new object();
        private volatile bool isDispose = false;
        private int awaitRcv = FALSE;
        
#if DEBUG
        private int c_count = 0;
        private int d_count = 0;
#endif
        
        
        public MemorySegment GetSegment()
        {
            if (this.awaitWriteSegment != null)
            {
                return this.awaitWriteSegment as MemorySegment;
            }

            MemorySegment segment;
            if (!this.segmentStack.TryPop(out segment))
            {
                segment = new MemorySegment(this.segmentStack);
                segment.SetMemoryBlock(this.memoryPool.Rent());

#if DEBUG
                Interlocked.Increment(ref c_count);
#endif
            }

            this.awaitWriteSegment = segment;
            return segment;
        }
        
        public void WriterAdvance()
        {
            if (awaitWriteSegment == null)
            {
                throw new NullReferenceException(nameof(awaitWriteSegment));
            }
            
            lock (this.sync)
            {
                var segment = this.awaitWriteSegment;
                this.awaitWriteSegment = null;
                
                if (this.headBufferSegment == null && 
                    this.tailBufferSegment == null)
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

        public ReadDgramResult Read()
        {
            lock (this.sync)
            {
                return new ReadDgramResult(this.headBufferSegment, this.tailBufferSegment);
            }
        }

        public void ReaderAdvance(int count)
        {
            lock (this.sync)
            {
                while (count > 0)
                {
                    if (this.headBufferSegment == null)
                    {
                        throw new ArgumentOutOfRangeException("The Segment is not enough");
                    }

                    var segment = this.headBufferSegment.ChildSegment;
                    this.headBufferSegment.Reset();
                    this.headBufferSegment = segment;
                    count--;

                    if (this.headBufferSegment == null)
                    {
                        this.headBufferSegment = null;
                        this.tailBufferSegment = null;
                    }
                }
            }
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

        public void Complete()
        {
            if (this.isDispose)
            {
                return;
            }

            this.isDispose = true;
            
            lock (this.sync)
            {
                if (this.awaitWriteSegment != null)
                {
                    this.awaitWriteSegment.Dispose();
#if DEBUG
                    Interlocked.Increment(ref d_count);
#endif
                }
                
                ASegment segment = this.headBufferSegment;
                while (segment != null)
                {
                    ASegment returnSegment = segment;
                    segment = segment.ChildSegment;

                    returnSegment.Dispose();
#if DEBUG
                    Interlocked.Increment(ref d_count);
#endif
                }
            }
            
            this.memoryPool = null;
            
            while (this.segmentStack.TryPop(out var item))
            {
                item.Dispose();
#if DEBUG
                Interlocked.Increment(ref d_count);
#endif
            }
            
            this.segmentStack.Clear();


#if DEBUG
            Logger.Info($"create buffer count:{this.c_count},    recycle buffer count:{this.d_count}");
#endif
        }
    }
}