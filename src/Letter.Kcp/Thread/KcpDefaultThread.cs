using System;
using System.Collections.Generic;
using System.Threading;

namespace Letter.Kcp
{
    public sealed class KcpDefaultThread : IKcpThread
    {
        private volatile bool isStop = false;
        
        private Thread thread;
        private HashSet<IKcpRunnable> runnables = new HashSet<IKcpRunnable>();
        
        public void Start()
        {
            thread = new Thread(this.Update);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Register(IKcpRunnable runnable)
        {
            if (runnable == null)
            {
                throw new ArgumentNullException(nameof(runnable));
            }
            
            this.runnables.Add(runnable);
        }

        public void Unregister(IKcpRunnable runnable)
        {
            if (thread == null)
            {
                throw new ArgumentNullException(nameof(runnable));
            }
            
            this.runnables.Remove(runnable);
        }
        
        private void Update(object state)
        {
            while (!this.isStop)
            {
                Thread.Sleep(1);
                DateTime nowTime = TimeHelpr.GetNowTime();
                foreach (var item in runnables)
                {
                    item.Update(ref nowTime);
                }
            }
        }
        
        public void Stop()
        {
            this.isStop = true;
            this.runnables.Clear();
        }
    }
}