namespace System.IO.Pipelines
{
    public sealed class WrappedPipelineReader
    {
        internal WrappedPipelineReader(WrappedPipeline pipe, PipeReader reader)
        {
            this.reader = reader;
            this.pipe = pipe;
        }

        private PipeReader reader;
        private WrappedPipeline pipe;

        internal PipeReader Reader => this.reader;
    }
}