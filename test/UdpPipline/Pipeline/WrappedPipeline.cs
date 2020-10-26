using System.Buffers;
using System.Threading.Tasks;

namespace System.IO.Pipelines
{
    public delegate void PipelineReaderDelegate(WrappedPipelineReader reader, ReadOnlySequence<byte> buffer);
    
    sealed class WrappedPipeline : IAsyncDisposable
    {
        public WrappedPipeline(PipeOptions options, PipelineReaderDelegate readerCallback)
        {
            this.pipe = new Pipe(options);
            this.wrappedReader = new WrappedPipelineReader(this, this.pipe.Reader);
            this.wrappedWriter = new WrappedPipelineWriter(this, this.pipe.Writer);
            
            this.readerCallback = readerCallback;

            this.Start();
        }
        
        private Pipe pipe;
        private WrappedPipelineReader wrappedReader;
        private WrappedPipelineWriter wrappedWriter;

        private PipelineReaderDelegate readerCallback;

        public WrappedPipelineReader WrappedReader => this.wrappedReader;
        public WrappedPipelineWriter WrappedWriter => this.wrappedWriter;

        private Task readTask;
        
        private void Start()
        {
            this.readTask = this.ReadAsync();
        }

        private async Task ReadAsync()  
        {
            var reader = this.WrappedReader.Reader;
            
            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                if (result.IsCanceled || result.IsCompleted)
                {
                    break;
                }
                
                this.readerCallback(this.WrappedReader, result.Buffer);
            }
            
            reader.Complete();
        }
        
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}