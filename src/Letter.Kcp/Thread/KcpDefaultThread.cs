using System;
using System.Collections.Generic;
using System.Threading;

namespace Letter.Kcp
{
    public sealed class KcpDefaultThread : IKcpThread
    {
        private bool isStop = false;
        
        private Thread thread;
        private HashSet<IKcpRunnable> runnables = new HashSet<IKcpRunnable>();
        
        public void Start()
        {
            thread = new Thread(Update);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Register(IKcpRunnable runnable)
        {
            if (thread == null)
            {
                throw new NullReferenceException("");
            }
            
            runnables.Add(runnable);
        }

        public void Unregister(IKcpRunnable runnable)
        {
            if (thread == null)
            {
                throw new NullReferenceException("");
            }
            
            runnables.Remove(runnable);
        }
        
        private void Update(object state)
        {
            while (!this.isStop)
            {
                Thread.Sleep(1);
                foreach (var item in runnables)
                {
                    item.Update();
                }
            }
        }
        
        public void Stop()
        {
            this.isStop = true;
        }
    }
}