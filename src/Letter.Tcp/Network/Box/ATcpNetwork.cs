namespace Letter.Tcp.Box
{
    abstract class ATcpNetwork<TTcpOptions> : Letter.Box.ANetwork<TTcpOptions>
        where TTcpOptions : ATcpOptions
    {
        protected ATcpNetwork(TTcpOptions options) : base(options)
        {
        }
    }
}