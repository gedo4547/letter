using System;
using System.Threading.Tasks;

namespace Letter.IO
{
    public interface IChannel<TSession, TOptions> 
        where TSession : ISession
        where TOptions : IOptions
    {
        void ConfigurationSelfOptions(TOptions options);
        void ConfigurationSelfFilter(Action<IFilterPipeline<TSession>> handler);
        Task StopAsync();
    }
}