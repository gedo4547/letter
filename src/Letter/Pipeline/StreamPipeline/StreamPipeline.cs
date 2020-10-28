using System;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.IO.Pipelines
{
    public sealed class StreamPipeline : IAsyncDisposable
    {
        public StreamPipeline(PipeOptions options)
        {
            this.tcpPipe = new Pipe(options);
            this.reader = new StreamPipelineReader(this.tcpPipe.Reader);
            this.writer = new StreamPipelineWriter(this.tcpPipe.Writer);
        }

        private Pipe tcpPipe;
        private StreamPipelineReader reader;
        private StreamPipelineWriter writer;

        public StreamPipelineReader Reader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.reader; }
        }

        public StreamPipelineWriter Writer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.writer; }
        }

        public ValueTask DisposeAsync()
        {
            this.reader = null;
            this.writer = null;

            return default;
        }
    }
}