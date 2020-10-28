using System;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Letter
{
    public sealed class StreamPipelineReader : PipeReader
    {
        public StreamPipelineReader(PipeReader reader)
        {
            this.reader = reader;
        }

        private PipeReader reader;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void AdvanceTo(SequencePosition consumed) => this.reader.AdvanceTo(consumed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined) 
            => this.reader.AdvanceTo(consumed, examined);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CancelPendingRead() => this.reader.CancelPendingRead();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Complete(Exception exception = null) => this.reader.Complete(exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = new CancellationToken())
            => this.reader.ReadAsync(cancellationToken);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryRead(out ReadResult result) => this.reader.TryRead(out result);
    }
}