namespace Letter.Udp
{
    public static class UdpFactory
    {
        public static IUdpClient Client()
        {
            return new UdpClient();
        }
    }
}