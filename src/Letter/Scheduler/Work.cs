namespace System.Threading
{
    readonly struct Work
    {
        public readonly Action<object> Callback;
        public readonly object State;

        public Work(Action<object> callback, object state)
        {
            Callback = callback;
            State = state;
        }
    }
}