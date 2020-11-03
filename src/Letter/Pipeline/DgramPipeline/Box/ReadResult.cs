namespace System.IO.Pipelines
{
    public ref struct ReadDgramResult
    {
        public bool Completed 
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