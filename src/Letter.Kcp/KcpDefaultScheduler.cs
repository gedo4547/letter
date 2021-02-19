using System;
using System.Collections.Generic;
using System.Threading;

namespace Letter.Kcp
{
    public sealed class KcpDefaultScheduler : IKcpScheduler
    {
        public KcpDefaultScheduler()
        {
            thread = new Thread(this.Update);
            thread.IsBackground = true;
            thread.Start();
        }

        private volatile bool isStop = false;
        
        private Thread thread;

        private HashSet<IKcpRunnable> runnables = new HashSet<IKcpRunnable>();

        public void Register(IKcpRunnable runnable)
        {
            if (this.isStop)
            {
                throw new Exception("");
            }
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

                try
                {
                    var enumerator = this.runnables.GetEnumerator();
                    while(enumerator.MoveNext())
                    {
                        enumerator.Current.Update();
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
            Console.WriteLine("Stop");
            this.isStop = true;
            this.runnables.Clear();
        }
    }
}