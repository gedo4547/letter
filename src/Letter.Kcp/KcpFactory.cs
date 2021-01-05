namespace Letter.Kcp
{
    public static class KcpFactory
    {
        public static IKcpBootstrap Bootstrap() => new KcpBootstrap();
        
        public static IKcpClientBootstrap ClientBootstrap() => new KcpClientBootstrap();

        public static IKcpServerBootstrap ServerBootstrap() => new KcpServerBootstrap();
    }
}