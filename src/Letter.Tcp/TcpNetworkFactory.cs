namespace Letter.Tcp
{
    public static class TcpNetworkFactory
    {
        public static ITcpListener Listener() => new TcpListener();

        public static ITcpConnector Connector() => new TcpConnector();
    }
}