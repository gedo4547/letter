using System.Buffers;
using UdpPipline;

namespace System.IO.Pipelines
{
    
    
    public sealed class PipelineGroup
    {
        public PipelineGroup(PipeOptions options)
        {
            this.options = options;
        }

        private PipeOptions options;

        private PipelineReaderDelegate readerCallback;

        public void AddReaderEvent(PipelineReaderDelegate readerCallback)
        {
            this.readerCallback = readerCallback;
        }


        public WrappedPipelineWriter GetWriter()
        {
            return default;
        }


    }
}