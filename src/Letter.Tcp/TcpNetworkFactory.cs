
namespace Letter.Tcp
{
    public static class TcpNetworkFactory
    {
        public static ITcpServer Server() => new TcpServer();
        public static ITcpClient Client() => new TcpClient();
    }
}