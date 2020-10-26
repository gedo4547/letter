using System;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Letter
{
    public sealed class TcpPipeline : IAsyncDisposable
    {
        public TcpPipeline(PipeOptions options)
        {
            this.tcpPipe = new Pipe(options);
            this.reader = new TcpPipelineReader(this.tcpPipe.Reader);
            this.writer = new TcpPipelineWriter(this.tcpPipe.Writer);
        }

        private Pipe tcpPipe;
        private TcpPipelineReader reader;
        private TcpPipelineWriter writer;

        public TcpPipelineReader Reader
        {
            get { return this.reader; }
        }

        public TcpPipelineWriter Writer
        {
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