using System;
using System.Net;
using System.Threading.Tasks;

namespace Letter
{
    public interface IBootstrap<TChannel, TSession, TOptions> 
        where TSession : ISession
        where TOptions : IOptions
        where TChannel : IChannel<TSession>
    {
        void Logger(ISocketsTrace trace);

        void Options(Action<TOptions> optionsFactory);
        
        void Channel(Func<TChannel> channelFactory);

        Task StartAsync(EndPoint point);
        
        Task StopAsync();
    }
}