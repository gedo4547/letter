using Letter.Tcp.Box;

namespace Letter.Tcp
{
    public static class TcpNetworkFactory
    {
        public static ITcpListener Listener() => new TcpListener();

        public static ITcpConnector Connector() => new TcpConnector();


        public static ITcpServer Server() => new TcpServer();
        public static ITcpClient Client() => new TcpClient();
    }
}