using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO.Pipelines;

namespace System.Threading
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class IOQueue : PipeScheduler
#if NET5_0
        , IThreadPoolWorkItem
#endif
    {
        public IOQueue()
        {
#if NETSTANDARD2_0
            this.execute_callBack = this.Execute;
#endif
        }
        
        
        private readonly ConcurrentQueue<Work> _workItems = new ConcurrentQueue<Work>();
        private int _doingWork;

#if NETSTANDARD2_0
        private readonly WaitCallback execute_callBack;
#endif
        
        
        public override void Schedule(Action<object> action, object state)
        {
            _workItems.Enqueue(new Work(action, state));

            // Set working if it wasn't (via atomic Interlocked).
            if (Interlocked.CompareExchange(ref _doingWork, 1, 0) == 0)
            {
#if NETSTANDARD2_0
                System.Threading.ThreadPool.UnsafeQueueUserWorkItem(this.execute_callBack, null);
#elif NET5_0
                System.Threading.ThreadPool.UnsafeQueueUserWorkItem(this, preferLocal: false);
#endif
            }
        }

#if NETSTANDARD2_0
        private void Execute(object state)
#elif NET5_0
        void IThreadPoolWorkItem.Execute()
#endif
        {
            while (true)
            {
                while (_workItems.TryDequeue(out Work item))
                {
                    item.Callback(item.State);
                }

                // All work done.

                // Set _doingWork (0 == false) prior to checking IsEmpty to catch any missed work in interim.
                // This doesn't need to be volatile due to the following barrier (i.e. it is volatile).
                _doingWork = 0;

                // Ensure _doingWork is written before IsEmpty is read.
                // As they are two different memory locations, we insert a barrier to guarantee ordering.
                Thread.MemoryBarrier();

                // Check if there is work to do
                if (_workItems.IsEmpty)
                {
                    // Nothing to do, exit.
                    break;
                }

                // Is work, can we set it as active again (via atomic Interlocked), prior to scheduling?
                if (Interlocked.Exchange(ref _doingWork, 1) == 1)
                {
                    // Execute has been rescheduled already, exit.
                    break;
                }

                // Is work, wasn't already scheduled so continue loop.
            }
        }

     
    }
}