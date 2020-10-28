using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Sockets
{
    public sealed class SocketAwaitableArgs : SocketAsyncEventArgs, ICriticalNotifyCompletion
    {
        private static readonly Action _callbackCompleted = () => { };
        
        public SocketAwaitableArgs(PipeScheduler ioScheduler)
#if NET5_0
            : base(unsafeSuppressExecutionContextFlow: true)
#endif
        {
            this.ioScheduler = ioScheduler;
            this.completedCallback = (o) => { ((Action) o)(); };
        }
        
        private Action _callback;
        
        private readonly PipeScheduler ioScheduler;
        private readonly Action<object> completedCallback;

        public SocketAwaitableArgs GetAwaiter() => this;
        public bool IsCompleted => ReferenceEquals(_callback, _callbackCompleted);

        public int GetResult()
        {
            _callback = null;

            if (SocketError != SocketError.Success)
            {
                throw new SocketException((int)SocketError);
            }
            
            return this.BytesTransferred;
        }

        public void OnCompleted(Action continuation)
        {
            if (ReferenceEquals(_callback, _callbackCompleted) ||
                ReferenceEquals(Interlocked.CompareExchange(ref _callback, continuation, null), _callbackCompleted))
            {
                Task.Run(continuation);
            }
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            OnCompleted(continuation);
        }

        public void Complete()
        {
            OnCompleted(this);
        }

        protected override void OnCompleted(SocketAsyncEventArgs _)
        {
            var continuation = Interlocked.Exchange(ref _callback, _callbackCompleted);

            if (continuation != null)
            {
                ioScheduler.Schedule(this.completedCallback, continuation);
            }
        }
    }
}