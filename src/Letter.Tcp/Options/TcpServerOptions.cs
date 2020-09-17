using System;
using System.Threading;

namespace Letter.Tcp
{
    public class TcpServerOptions : ATcpOptions
    {
        /// <summary>
        /// The number of I/O queues used to process requests. Set to 0 to directly schedule I/O to the ThreadPool.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Environment.ProcessorCount" /> rounded down and clamped between 1 and 16.
        /// </remarks>
        public int IOQueueCount { get; set; } = Math.Min(Environment.ProcessorCount, 16);
        
        /// <summary>
        /// The maximum length of the pending connection queue.
        /// </summary>
        /// <remarks>
        /// Defaults to 512.
        /// </remarks>
        public int Backlog { get; set; } = 512;
        
        public int SchedulerCount
        {
            get { return SchedulerAllocator.shared.Count;}
            set
            {
                int count = value;
                if (count == SchedulerAllocator.shared.Count)
                    return;
                this.Allocator = new SchedulerAllocator(count);
            }
        }
        
        internal SchedulerAllocator Allocator { get; private set; } = SchedulerAllocator.shared;
    }
}