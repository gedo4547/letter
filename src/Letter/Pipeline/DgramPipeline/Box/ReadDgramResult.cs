namespace System.IO.Pipelines
{
    public ref struct ReadDgramResult
    {
        public ReadDgramResult(ASegment headBufferSegment, ASegment tailBufferSegment)
        {
            
        }

        public bool IsCompleted 
        {
            get
            {
                return true;
            }
        }

        public SegmentSequence GetBuffer()
        {
            return default;
        }
    }
}