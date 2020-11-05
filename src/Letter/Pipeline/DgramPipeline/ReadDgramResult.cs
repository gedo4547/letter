namespace System.IO.Pipelines
{
    public struct ReadDgramResult
    {
        public ReadDgramResult(ASegment headBufferSegment, ASegment tailBufferSegment)
        {
            this.IsEmpty = (headBufferSegment == null);
            this.headBufferSegment = headBufferSegment;
            this.tailBufferSegment = tailBufferSegment;
        }

        private ASegment headBufferSegment;
        private ASegment tailBufferSegment;

        public bool IsEmpty
        {
            get;
            private set;
        }

        public SegmentSequence GetBuffer()
        {
            if (!this.IsEmpty)
            {
                return new SegmentSequence(this.headBufferSegment, this.tailBufferSegment);
            }
            
            return default;
        }
    }
}