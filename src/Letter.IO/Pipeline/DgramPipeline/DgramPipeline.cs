using System.Buffers;
using System.Collections.Concurrent;
using System.Threading;

namespace System.IO.Pipelines
{
    public class DgramPipeline : IDisposable
    {
        private const int FALSE = 0;
        private const int TRUE = 1;
        
        private const int INIT_SEGMENT_POOL_SIZE = 8;

        public DgramPipeline(MemoryPool<byte> memoryPool, PipeScheduler scheduler, Action rcvCallback)
        {
            this.scheduler = scheduler;
            this.memoryPool = memoryPool;
            this.memoryBlockSize = this.memoryPool.MaxBufferSize;
            //this.segmentStack = new ConcurrentBufferStack<MemorySegment>(INIT_SEGMENT_POOL_SIZE);
            this.segmentStack1 = new ConcurrentStack<MemorySegment>();

            this.rcvCallback = (o) => { rcvCallback(); };
            this.Reader = new DgramPipelineReader(this);
            this.Writer = new DgramPipelineWriter(this);
        }

        public DgramPipelineReader Reader { get; private set; }
        public DgramPipelineWriter Writer { get; private set; }
        
        private PipeScheduler scheduler;
        private MemoryPool<byte> memoryPool;
        private ConcurrentBufferStack<MemorySegment> segmentStack;

        private ConcurrentStack<MemorySegment> segmentStack1;


        private Action<object> rcvCallback;
        
        private int memoryBlockSize;
        private ASegment headBufferSegment = null;
        private ASegment tailBufferSegment = null;
        
        private object sync = new object();
        
        private int awaitRcv = FALSE;

        public MemorySegment GetSegment()
        {
            MemorySegment segment;
            if (!this.segmentStack1.TryPop(out segment))
            {
                segment = new MemorySegment(this.segmentStack1);
                segment.SetMemoryBlock(this.memoryPool.Rent());
            }

            return segment;
        }
        
        public void WriterAdvance(ASegment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException(nameof(segment));
            }
            
            lock (this.sync)
            {
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
                var result = new ReadDgramResult(this.headBufferSegment, this.tailBufferSegment);
                
                this.headBufferSegment = null;
                this.tailBufferSegment = null;
                return result;
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

        public void Dispose()
        {
            Console.WriteLine("DgramPipeline.Dispose");
            lock (this.sync)
            {
                if (this.headBufferSegment != null)
                {
                    this.headBufferSegment.Dispose();
                    this.headBufferSegment = null;
                }

                if (this.tailBufferSegment != null)
                {
                    this.tailBufferSegment.Dispose();
                    this.tailBufferSegment = null;
                }
            }
        
            
            this.memoryPool = null;
            Logger.Error("FFFFFFFFFF>>" + segmentStack1.Count);
            try
            {
                while (this.segmentStack1.TryPop(out var item))
                {
                    Console.WriteLine("FFFFFFFFFF>>" + segmentStack1.Count);
                    Console.WriteLine("XXXXXXXXXXXXXXX>>");
                    item.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
           

            this.segmentStack1.Clear();
        }
    }
}