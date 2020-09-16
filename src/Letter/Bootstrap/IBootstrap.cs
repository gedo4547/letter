using System;
using System.Threading.Tasks;

namespace Letter
{
    public interface IBootstrap<TOptions, TFilter, TContext, TReader, TWriter>
        where TOptions: IOptions
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TFilter : IFilter<TContext, TReader, TWriter>
    {
        void AddFilter(Func<TFilter> FilterFactory);

        void ConfigurationOptions(Action<TOptions> optionsFactory);

        Task StopAsync();
    }
}