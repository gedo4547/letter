namespace System.IO.Pipelines
{
    public struct ReadDgramResult
    {
        public ReadDgramResult(ASegment headBufferSegment, ASegment tailBufferSegment)
        {
            this.IsEmpty = (headBufferSegment == null);
            this.Head = headBufferSegment;
            this.Tail = tailBufferSegment;
        }

        public ASegment Head { get; }
        public ASegment Tail { get; }

        public bool IsEmpty
        {
            get;
            private set;
        }
    }
}