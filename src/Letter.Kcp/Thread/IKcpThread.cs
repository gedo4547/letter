namespace Letter.Kcp
{
    public interface IKcpThread
    {
        void Start();
        void Stop();
        
        void Register(IKcpRunnable runnable);
        void Unregister(IKcpRunnable runnable);
    }
}