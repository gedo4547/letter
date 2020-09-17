namespace Letter.Tcp
{
    public abstract class ATcpBootstrap<TOptions, TChannel> : AStreamBootstrap<TOptions, ITcpSession, ITcpChannelFilter, TChannel>, ITcpBootstrap<TOptions, TChannel>
        where TOptions : ATcpOptions, new() 
        where TChannel : IChannel
    {
        
    }
}