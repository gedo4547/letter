namespace System.IO.Pipelines
{
    public ref struct SegmentSequence
    {
        public struct Enumerator
        {
            public Enumerator(ASegment headBufferSegment, ASegment tailBufferSegment)
            {
                this.isLastSegment = false;
                this.current = null;
                this.headBufferSegment = headBufferSegment;
                this.tailBufferSegment = tailBufferSegment;
            }

            private ASegment current;
            private ASegment headBufferSegment;
            private ASegment tailBufferSegment;

            private bool isLastSegment;
            
            public ASegment Current
            {
                get
                {
                    return this.current;
                }
            }

            public bool MoveNext()
            {
                if (!this.isLastSegment)
                {
                    if (this.current == null)
                    {
                        this.current = this.headBufferSegment;
                    }
                    else
                    {
                        this.current = this.current.ChildSegment;
                    }
                    
                    if (this.current == this.tailBufferSegment)
                    {
                        isLastSegment = true;
                    }

                    return true;
                }

                return false;
            }
        }

        public SegmentSequence(ASegment headBufferSegment, ASegment tailBufferSegment)
        {
            this.headBufferSegment = headBufferSegment;
            this.tailBufferSegment = tailBufferSegment;
        }

        private ASegment headBufferSegment;
        private ASegment tailBufferSegment;
        
        public Enumerator GetEnumerator()
        {
            if (headBufferSegment == null)
            {
                throw new Exception("buffers are empty");
            }

            return new Enumerator(this.headBufferSegment, this.tailBufferSegment);
        }
    }
}