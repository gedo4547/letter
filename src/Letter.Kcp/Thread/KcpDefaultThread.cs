using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;

namespace Letter.Kcp
{
    public sealed class KcpDefaultThread : IKcpThread
    {
        private volatile bool isStop = false;
        
        private Thread thread;

        //System.Collections.Concurrent.<string> ts = new System.Collections.Concurrent.ConcurrentBag<string>();

        private HashSet<IKcpRunnable> runnables = new HashSet<IKcpRunnable>();
        
        public void Start()
        {
            thread = new Thread(this.Update);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Register(IKcpRunnable runnable)
        {
            
            Console.WriteLine("     Register   ");
            if (runnable == null)
            {
                throw new ArgumentNullException(nameof(runnable));
            }
            
            this.runnables.Add(runnable);
        }

        public void Unregister(IKcpRunnable runnable)
        {
            Logger.Error("     Unregister   ");
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
                try
                {
                    foreach (var item in runnables)
                    {
                        item.Update(ref nowTime);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
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