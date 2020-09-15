namespace Letter
{
    public interface IDgramBootstrap<TOptions, TSession, TChannel, TNetwork> : IBootstrap<TOptions, TNetwork>
        where TOptions : class, IOptions, new()
        where TSession : ISession
        where TChannel : IDgramChannel<TSession>
        where TNetwork : IDgramNetwork<TSession, TChannel>
    {
        
    }
}