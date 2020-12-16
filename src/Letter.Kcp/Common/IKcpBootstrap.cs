using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpBootstrap : IBootstrap<KcpOptions, IKcpSession, IKcpChannel>
    {
        void ConfigurationGlobalScheduler(IKcpScheduler scheduler);
    }
}