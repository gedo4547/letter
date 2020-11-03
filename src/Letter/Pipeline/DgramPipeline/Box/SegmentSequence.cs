namespace System.IO.Pipelines
{
    public struct SegmentSequence
    {
        public struct Enumerator
        {
            public Enumerator(ASegment headBufferSegment, ASegment tailBufferSegment)
            {
                this.current = null;
                this.headBufferSegment = headBufferSegment;
                this.tailBufferSegment = tailBufferSegment;
            }

            private ASegment current;
            private ASegment headBufferSegment;
            private ASegment tailBufferSegment;
            
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
                if (this.current == null)
                {
                    this.current = this.headBufferSegment;
                    return true;
                }

                return false;
            }
        }


        public Enumerator GetEnumerator()
        {
            return new Enumerator();
        }
    }
}