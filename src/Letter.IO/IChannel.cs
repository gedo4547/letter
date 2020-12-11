using System;
using System.Threading.Tasks;

namespace Letter.IO
{
    public interface IChannel<TSession, TOptions> 
        where TSession : ISession
        where TOptions : IOptions
    {
        void SettingOptions(TOptions options);
        void AddFilter(IFilter<TSession> filter);
        
        Task StopAsync();
    }
}