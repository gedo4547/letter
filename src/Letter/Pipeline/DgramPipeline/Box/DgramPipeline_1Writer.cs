using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    public sealed class DgramPipeline_1Writer
    {
        public DgramPipeline_1Writer(DgramPipeline_1 pipeline)
        {
            this.pipeline = pipeline;
        }

        private DgramPipeline_1 pipeline;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemorySegment GetSegment() => this.pipeline.GetSegment();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FlushAsync() => this.pipeline.FlushAsync();
    }
}