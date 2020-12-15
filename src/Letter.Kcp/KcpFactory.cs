namespace Letter.Kcp
{
    public static class KcpFactory
    {
        public static IKcpClientBootstrap ClientBootstrap() => new KcpClientBootstrap();

        public static IKcpServerBootstrap ServerBootstrap() => new KcpServerBootstrap();
    }
}