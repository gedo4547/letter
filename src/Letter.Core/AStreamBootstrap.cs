namespace Letter.Box.ssss
{
    public abstract class AStreamBootstrap<TOptions, TSession, TChannel, TNetwork> : ABootstrap<TOptions, TNetwork>, IStreamBootstrap<TOptions, TSession, TChannel, TNetwork>
        where TOptions : IOptions, new()
        where TSession : ISession
        where TChannel : IStreamChannel<TSession>
        where TNetwork : IStreamNetwork<TSession, TChannel>
    {
    }
}