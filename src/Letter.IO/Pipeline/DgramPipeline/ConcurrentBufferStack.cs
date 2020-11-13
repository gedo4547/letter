namespace System.IO.Pipelines
{
    public sealed class ConcurrentBufferStack<T>
    {
        public ConcurrentBufferStack(int size)
        {
        


            this.stack = new BufferStack<T>(size);
        }

        private BufferStack<T> stack;
        private object sync = new object();

        public int Count
        {
            get { return this.stack.Count; }
        }

        public bool TryPop(out T value)
        {
            lock (this.sync)
            {
                return this.stack.TryPop(out value);
            }
        }

        public void Push(T value)
        {
            lock (this.sync)
            {
                this.stack.Push(value);
            }
        }
    }
}