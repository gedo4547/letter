using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines
{
    public sealed class StreamPipelineWriter : PipeWriter, IWrappedWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StreamPipelineWriter(PipeWriter writer) => this.writer = writer;

        private PipeWriter writer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Complete(Exception exception = null) => this.writer.Complete(exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CancelPendingFlush() => this.writer.CancelPendingFlush();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = new CancellationToken())
            => this.writer.FlushAsync(cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Advance(int bytes) => this.writer.Advance(bytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Memory<byte> GetMemory(int sizeHint = 0) => this.writer.GetMemory(sizeHint);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Span<byte> GetSpan(int sizeHint = 0) => this.writer.GetSpan(sizeHint);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetWritableSpan(int length) => this.writer.GetSpan(length);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> GetWritableMemory(int length) => this.writer.GetMemory(length);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriterAdvance(int length) => this.writer.Advance(length);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in ReadOnlySpan<byte> span) => this.writer.Write(span);
    }
}