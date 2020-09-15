namespace Letter.Box.ssss
{
    public interface IDgramBootstrap<TOptions, TSession, TChannel, TNetwork> : IBootstrap<TOptions, TNetwork>
        where TOptions : IOptions, new()
        where TSession : ISession
        where TChannel : IDgramChannel<TSession>
        where TNetwork : IDgramNetwork<TSession, TChannel>
    {
        
    }
}