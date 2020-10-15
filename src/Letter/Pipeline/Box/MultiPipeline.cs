using System.Buffers;
using System.IO.Pipelines;

namespace Letter
{
    public sealed class MultiPipeline
    {
        public MultiPipeline(MemoryPool<byte> pool = null, PipeScheduler readerScheduler = null, PipeScheduler writerScheduler = null, long pauseWriterThreshold = -1, long resumeWriterThreshold = -1, int minimumSegmentSize = -1, bool useSynchronizationContext = true)
        {
            this.pool = pool;
            this.readerScheduler = readerScheduler;
            this.writerScheduler = writerScheduler;
            this.pauseWriterThreshold = pauseWriterThreshold;
            this.resumeWriterThreshold = resumeWriterThreshold;
            this.minimumSegmentSize = minimumSegmentSize;
            this.useSynchronizationContext = useSynchronizationContext;
        }

        private MemoryPool<byte> pool;
        private PipeScheduler readerScheduler;
        private PipeScheduler writerScheduler;
        private long pauseWriterThreshold;
        private long resumeWriterThreshold;
        private int minimumSegmentSize;
        private bool useSynchronizationContext;

        
        
        
        
        
        
        
        
        

    }
}