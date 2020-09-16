using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ABootstrap<TOptions, TFilterGroup, TFilter, TContext, TReader, TWriter> : IBootstrap<TOptions, TFilter, TContext, TReader, TWriter>
        where TOptions: IOptions
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TFilter : IFilter<TContext, TReader, TWriter>
        where TFilterGroup : AFilterGroup<TFilter, TContext, TReader, TWriter>
    {
        public ABootstrap()
        {
            this.FilterGroupFactory = new FilterGroupFactory<TFilterGroup, TFilter, TContext, TReader, TWriter>(this.OnCreateFilterGroup);
        }
        
        protected Action<TOptions> optionsFactory;
        protected FilterGroupFactory<TFilterGroup, TFilter, TContext, TReader, TWriter> FilterGroupFactory;
        
        public void AddFilter(Func<TFilter> FilterFactory)
        {
            if (FilterFactory == null)
                throw new ArgumentNullException(nameof(FilterFactory));

            this.FilterGroupFactory.AddFilterFactory(FilterFactory);
        }

        public void ConfigurationOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
                throw new ArgumentNullException(nameof(optionsFactory));

            this.optionsFactory = optionsFactory;
        }
        
        protected abstract TFilterGroup OnCreateFilterGroup(List<TFilter> Filters);
        
        public virtual Task StopAsync()
        {
            this.optionsFactory = null;
            
            return Task.CompletedTask;
        }
    }
}