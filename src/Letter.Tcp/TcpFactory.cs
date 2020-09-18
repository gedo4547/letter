
namespace Letter.Tcp
{
    public static class TcpFactory
    {
        public static ITcpServer Server() => new TcpServer();
        public static ITcpClient Client() => new TcpClient();
        
        public static ITcpClientBootstrap ClientBootstrap() => new TcpClientBootstrap();
        // public static ITcpServerBootstrap ServerBootstrap() => new TcpServerBootstrap();
    }
}