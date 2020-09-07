namespace Letter.Tcp
{
    abstract class ATcpNetwork<TTcpOptions> : ANetwork<TTcpOptions>
        where TTcpOptions : ATcpOptions
    {
        protected ATcpNetwork(TTcpOptions options) : base(options)
        {
        }
    }
}