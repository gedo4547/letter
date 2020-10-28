using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace Letter.Tcp
{
    internal class DuplexPipe : IWrappedDuplexPipe
    {
        public DuplexPipe(StreamPipelineReader reader, StreamPipelineWriter writer)
        {
            WrappedInput = reader;
            WrappedOutput = writer;
        }

        public StreamPipelineReader WrappedInput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public StreamPipelineWriter WrappedOutput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
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
        

        public static DuplexPipePair CreateConnectionPair(PipeOptions inputOptions, PipeOptions outputOptions)
        {
            var input = new StreamPipeline(inputOptions);
            var output = new StreamPipeline(outputOptions);

            var transportToApplication = new DuplexPipe(output.Reader, input.Writer);
            var applicationToTransport = new DuplexPipe(input.Reader, output.Writer);

            return new DuplexPipePair(applicationToTransport, transportToApplication);
        }

        // This class exists to work around issues with value tuple on .NET Framework
        public readonly struct DuplexPipePair
        {
            public IWrappedDuplexPipe Transport
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get;
            }

            public IWrappedDuplexPipe Application
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get;
            }

            public DuplexPipePair(IWrappedDuplexPipe transport, IWrappedDuplexPipe application)
            {
                Transport = transport;
                Application = application;
            }
        }
    }
}