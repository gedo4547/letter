namespace Letter.Kcp
{
    public static class KcpFactory
    {
        public static IKcpBootstrap Bootstrap() => new KcpBootstrap();
    }
}