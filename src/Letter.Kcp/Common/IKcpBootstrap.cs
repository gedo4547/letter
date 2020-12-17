using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpBootstrap : IBootstrap<KcpOptions, IKcpSession, IKcpChannel>
    {
        void ConfigurationGlobalThread(IKcpThread thread);
    }
}