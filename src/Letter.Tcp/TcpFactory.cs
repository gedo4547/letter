
namespace Letter.Tcp
{
    public static class TcpFactory
    {
        public static ITcpClientBootstrap ClientBootstrap() => new TcpClientBootstrap();
        public static ITcpServerBootstrap ServerBootstrap() => new TcpServerBootstrap();
    }
}