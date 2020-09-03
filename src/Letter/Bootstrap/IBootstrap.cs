using System;
using System.Net;
using System.Threading.Tasks;

namespace Letter
{
    public interface IBootstrap<TTransport, TContext, TSession, TOptions, TChannel, TReader, TWriter>
        where TContext : class, IContext
        where TReader : struct
        where TWriter : struct
        where TOptions : IOptions
        where TSession : ISession
        where TChannel : IChannel<TContext, TReader, TWriter>
        where TTransport : ITransport<TSession, TChannel, TContext, TReader, TWriter>
    {
        void ConfigureOptions(Action<TOptions> optionsFactory);

        void ConfigureTransport(Func<TTransport> transportFactory);
        
        void ConfigureChannel(Func<TChannel> channelFactory);

        Task StartAsync(EndPoint point);
        
        Task StopAsync();
    }
}