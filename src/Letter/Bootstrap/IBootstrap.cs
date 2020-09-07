using System;
using System.Threading.Tasks;

namespace Letter
{
    public interface IBootstrap<TOptions, TChannel, TContext, TReader, TWriter>
        where TOptions: IOptions
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TChannel : IChannel<TContext, TReader, TWriter>
    {
        void AddChannel(Func<TChannel> channelFactory);

        void ConfigurationOptions(Action<TOptions> optionsFactory);

        Task StopAsync();
    }
}