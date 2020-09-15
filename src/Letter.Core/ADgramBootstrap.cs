namespace Letter.Box.ssss
{
    public abstract class ADgramBootstrap<TOptions, TSession, TChannel, TNetwork> : ABootstrap<TOptions, TNetwork>, IDgramBootstrap<TOptions, TSession, TChannel, TNetwork>
        where TOptions : IOptions, new()
        where TSession : ISession
        where TChannel : IDgramChannel<TSession>
        where TNetwork : IDgramNetwork<TSession, TChannel>
    {
        
    }
}