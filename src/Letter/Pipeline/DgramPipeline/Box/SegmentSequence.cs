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
                    Console.WriteLine("11111111111111111");
                    return this.current;
                }
            }

            public bool MoveNext()
            {
                //这里先走
                Console.WriteLine("2222222222222222222222222");
                if (!this.isLastSegment)
                {
                    this.current = this.headBufferSegment;
                    if (this.current == this.tailBufferSegment)
                    {
                        isLastSegment = true;
                    }
                    else
                    {
                        this.headBufferSegment = this.headBufferSegment.ChildSegment;
                    }
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
            return new Enumerator(this.headBufferSegment, this.tailBufferSegment);
        }
    }
}