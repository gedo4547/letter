namespace Letter.Box.ssss
{
    public interface IStreamBootstrap<TOptions, TSession, TChannel, TNetwork> : IBootstrap<TOptions, TNetwork>
        where TOptions : IOptions, new()
        where TSession : ISession
        where TChannel : IStreamChannel<TSession>
        where TNetwork : IStreamNetwork<TSession, TChannel>
    {
        
    }
}