namespace Letter.Box
{
    internal delegate void WriterFlushDelegate();
    
    public ref struct WrappedWriter
    {
        internal WrappedWriter(BinaryOrder order, WriterFlushDelegate writerFlush)
        {
            this.writerFlush = writerFlush;
            this.operators = BinaryOrderOperatorsFactory.GetOperators(order);
        }

        private WriterFlushDelegate writerFlush;
        private readonly IBinaryOrderOperators operators; 
    }
}