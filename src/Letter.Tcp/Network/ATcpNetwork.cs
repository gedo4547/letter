namespace Letter.Tcp
{
    abstract class ATcpNetwork<TTcpOptions> : ANetwork<TTcpOptions>, ITcpNetwork<TTcpOptions>
        where TTcpOptions : ATcpOptions
    {
        public ATcpNetwork(TTcpOptions options) : base(options)
        {
            
        }
    }
}