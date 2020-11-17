using System;
using System.IO;
using System.IO.Pipelines;
using System.Net.Security;

namespace Letter.Tcp
{
    internal class SslStreamDuplexPipe : DuplexPipeStreamAdapter<SslStream>
    {
        public SslStreamDuplexPipe(
            IDuplexPipe transport,
            StreamPipeReaderOptions readerOptions, 
            StreamPipeWriterOptions writerOptions, 
            Func<Stream, SslStream> factory)
            : base(transport, readerOptions, writerOptions, factory)
        {
        }
    }
}
