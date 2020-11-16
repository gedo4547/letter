using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    public sealed class DgramPipelineWriter
    {
        public DgramPipelineWriter(DgramPipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        private DgramPipeline pipeline;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemorySegment GetSegment() => this.pipeline.GetSegment();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriterAdvance() => this.pipeline.WriterAdvance();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FlushAsync() => this.pipeline.FlushAsync();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete() => this.pipeline.Complete();
    }
}