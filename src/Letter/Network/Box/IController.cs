using System;

namespace Letter.Box
{
    public interface IController<TClient, TOptions>
        where TClient : IClient<TOptions>
        where TOptions : class, IOptions
    {
        void OnActive(TClient session);
        void OnInactive(TClient session);
        void OnException(TClient session, Exception ex);
    }
}