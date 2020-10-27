using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Letter
{
    public class TcpPipelineWriter : PipeWriter, IWrappedWriter
    {
        public TcpPipelineWriter(PipeWriter writer) => this.writer = writer;

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
        public void Write(in ReadOnlySpan<byte> span) => this.writer.Write(span);
    }
}