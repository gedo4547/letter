using System.Collections.Generic;

namespace System.IO.Pipelines
{
    public sealed class PipelineGroup
    {
        public PipelineGroup(PipeOptions options, PipelineReaderDelegate readerCallback)
        {
            this.options = options;
            this.readerCallback = readerCallback;
            this.pool = new PipelinePool(this.OnCreatePipeline);
        }

        private PipelinePool pool;
        private PipeOptions options;
        private PipelineReaderDelegate readerCallback;
        
        private HashSet<WrappedPipeline> useingPipelines = new HashSet<WrappedPipeline>();
        
        private object sync = new object();

        public WrappedPipelineWriter GetWriter()
        {
            lock (sync)
            {
                var pipeline = this.pool.Get();
                this.useingPipelines.Add(pipeline);
                
                return pipeline.WrappedWriter;
            }
        }
        
        private WrappedPipeline OnCreatePipeline()
        {
            return new WrappedPipeline(this.options, this.readerCallback);
        }

        class PipelinePool
        {
            public PipelinePool(Func<WrappedPipeline> createFunc)
            {
                this.queue = new Queue<WrappedPipeline>();
                this.createFunc = createFunc;
            }

            private Queue<WrappedPipeline> queue;
            private Func<WrappedPipeline> createFunc;

            public WrappedPipeline Get()
            {
                if (this.queue.Count > 0)
                {
                    return this.queue.Dequeue();
                }
                else
                {
                    return this.createFunc();
                }
            }

            public void Recycle(WrappedPipeline pipe)
            {
                this.queue.Enqueue(pipe);
            }
        }
    }
}