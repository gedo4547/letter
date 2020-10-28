using System.IO.Pipelines;

namespace System.IO.Pipelines
{
    public interface IWrappedDuplexPipe : IDuplexPipe
    {
        StreamPipelineReader WrappedInput { get; }
        StreamPipelineWriter WrappedOutput { get; }
    }
}