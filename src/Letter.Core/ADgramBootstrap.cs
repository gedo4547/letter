namespace Letter
{
    public abstract class ADgramBootstrap<TOptions, TSession, TChannel, TNetwork> : ABootstrap<TOptions, TNetwork>, IDgramBootstrap<TOptions, TSession, TChannel, TNetwork>
        where TOptions : class, IOptions, new()
        where TSession : ISession
        where TChannel : IDgramChannel<TSession>
        where TNetwork : IDgramNetwork<TSession, TChannel>
    {
        
    }
}