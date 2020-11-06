namespace Letter.Udp
{
    public static class UdpFactory
    {
        public static IUdpBootstrap Bootstrap() => new UdpBootstrap();
    }
}