using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    public sealed class DgramPipeline_1Reader
    {
        public DgramPipeline_1Reader(DgramPipeline_1 pipeline)
        {
            this.pipeline = pipeline;
        }

        private DgramPipeline_1 pipeline;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReceiveAsync() => this.pipeline.ReceiveAsync();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadDgramResult Read() => this.pipeline.Read();
    }
}