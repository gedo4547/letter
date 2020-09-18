namespace Letter.Tcp
{
    public interface ITcpBootstrap<TOptions, TChannel> : IStreamBootstrap<TOptions, ITcpSession, ITcpChannelFilter, TChannel>
        where TOptions : ATcpOptions, new()
        where TChannel : IChannel
    {
        
    }
}