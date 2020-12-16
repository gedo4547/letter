namespace Letter.Kcp
{
    public interface IKcpScheduler
    {
        void Start();
        void Stop();
        
        void Register(IKcpRunnable runnable);
        void Unregister(IKcpRunnable runnable);
    }
}