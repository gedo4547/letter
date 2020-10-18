namespace System.IO.Pipelines
{
    public sealed class WrappedPipelineWriter
    {
        internal WrappedPipelineWriter(WrappedPipeline pipe, PipeWriter writer)
        {
            this.writer = writer;
            this.pipe = pipe;
        }

        private PipeWriter writer;
        private WrappedPipeline pipe;

        public PipeWriter Writer => this.writer;
    }
}