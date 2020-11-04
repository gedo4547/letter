using System.Buffers;
using System.IO.Pipelines;

namespace Letter
{
    static class StreamPipeOptionsHelper
    {
        public static StreamPipeReaderOptions ReaderOptionsCreator(MemoryPool<byte> memoryPool)
        {
            var inputPipeOptions = new StreamPipeReaderOptions
            (
                pool: memoryPool,
                bufferSize: memoryPool.MaxBufferSize,
                minimumReadSize: memoryPool.MaxBufferSize / 2,
                leaveOpen: true
            );

            return inputPipeOptions;
        }

        public static StreamPipeWriterOptions WriterOptionsCreator(MemoryPool<byte> memoryPool)
        {
            var outputPipeOptions = new StreamPipeWriterOptions
            (
                pool: memoryPool,
                leaveOpen: true
            );

            return outputPipeOptions;
        }
    }
}
