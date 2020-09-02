namespace Letter.Tcp
{
    public interface ITcpBootstrap<TTcpOptions> : IBootstrap<ITcpChannel, ITcpSession, TTcpOptions>
        where TTcpOptions : ATcpOptions
    {
        
    }
}