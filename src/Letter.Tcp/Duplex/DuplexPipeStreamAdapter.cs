using System;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    /// <summary>
    /// A helper for wrapping a Stream decorator from an <see cref="IDuplexPipe"/>.
    /// </summary>
    /// <typeparam name="TStream"></typeparam>
    internal class DuplexPipeStreamAdapter<TStream> : DuplexPipeStream, IWrappedDuplexPipe where TStream : Stream
    {
        private bool _disposed;
        private readonly object _disposeLock = new object();

        public DuplexPipeStreamAdapter(IDuplexPipe duplexPipe, Func<Stream, TStream> createStream) 
            : this(duplexPipe, new StreamPipeReaderOptions(leaveOpen: true), new StreamPipeWriterOptions(leaveOpen: true), createStream)
        {
        }

        public DuplexPipeStreamAdapter(IDuplexPipe duplexPipe, StreamPipeReaderOptions readerOptions, StreamPipeWriterOptions writerOptions, Func<Stream, TStream> createStream) 
            : base(duplexPipe.Input, duplexPipe.Output)
        {
            var stream = createStream(this);
            Stream = stream;
            
            this.WrappedInput = new StreamPipelineReader(PipeReader.Create(stream, readerOptions));
            this.WrappedOutput = new StreamPipelineWriter(PipeWriter.Create(stream, writerOptions));
        }

        public TStream Stream { get; }

        public StreamPipelineReader WrappedInput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
        }

        public StreamPipelineWriter WrappedOutput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
        }

        
        public PipeReader Input
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get { return this.WrappedInput; }
        }
        
        public PipeWriter Output
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.WrappedOutput; }
        }
        
        
        

        public override ValueTask DisposeAsync()
        {
            lock (_disposeLock)
            {
                if (_disposed)
                {
                    return default;
                }
                _disposed = true;
            }
            
            this.Stream.Dispose();
            return base.DisposeAsync();
        }

      
    }
}
