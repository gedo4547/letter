namespace Letter.Kcp
{
    public interface IKcpScheduler
    {
        void Register(IKcpRunnable runnable);
        void Unregister(IKcpRunnable runnable);

        void Stop();
    }
}