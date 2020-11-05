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
        public void WriterAdvance(ASegment segment) => this.pipeline.WriterAdvance(segment);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FlushAsync() => this.pipeline.FlushAsync();
    }
}