using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    public sealed class DgramPipelineReader
    {
        public DgramPipelineReader(DgramPipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        private DgramPipeline pipeline;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReceiveAsync() => this.pipeline.ReceiveAsync();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadDgramResult Read() => this.pipeline.Read();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReaderAdvance(int count) => this.pipeline.ReaderAdvance(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete() => this.pipeline.Complete();
    }
}