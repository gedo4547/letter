﻿using System.IO.Pipelines;

namespace System.Threading
{
    public sealed class SchedulerAllocator
    {
        public static SchedulerAllocator threadPool = new SchedulerAllocator(0);
        public static SchedulerAllocator kestrel = new SchedulerAllocator(Math.Min(Environment.ProcessorCount, 16));
        public static SchedulerAllocator processor = new SchedulerAllocator(Environment.ProcessorCount * 2);
        
        public SchedulerAllocator(int count)
        {
            var ioQueueCount = count;
            if (ioQueueCount > 0)
            {
                numSchedulers = ioQueueCount;
                schedulers = new IOQueue[numSchedulers];

                for (var i = 0; i < numSchedulers; i++)
                {
                    schedulers[i] = new IOQueue();
                }
            }
            else
            {
                var directScheduler = new PipeScheduler[] { PipeScheduler.ThreadPool };
                numSchedulers = directScheduler.Length;
                schedulers = directScheduler;
            }
        }

        
        private int numSchedulers;
        private int schedulerIndex;
        private PipeScheduler[] schedulers;

        public int Count
        {
            get { return this.schedulers.Length; }
        }


        public PipeScheduler Next()
        {
            var scheduler = this.schedulers[this.schedulerIndex];
            this.schedulerIndex = (this.schedulerIndex + 1) % this.numSchedulers;
            return scheduler;
        }
    }
}