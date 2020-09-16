using System;
using System.Collections.Generic;

namespace Letter
{
    public sealed class FilterGroupFactory<TFilterGroup, TFilter, TContext, TReader, TWriter> : IDisposable
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TFilter : IFilter<TContext, TReader, TWriter>
        where TFilterGroup : AFilterGroup<TFilter, TContext, TReader, TWriter>
    {
        public FilterGroupFactory(Func<List<TFilter>, TFilterGroup> FilterGroupCreator)
        {
            if (FilterGroupCreator == null)
            {
                throw new ArgumentNullException(nameof(FilterGroupCreator));
            }

            this.FilterGroupCreator = FilterGroupCreator;
        }
        
        private Func<List<TFilter>, TFilterGroup> FilterGroupCreator;
        private List<Func<TFilter>> FilterFactorys = new List<Func<TFilter>>();

        public void AddFilterFactory(Func<TFilter> FilterFactory)
        {
            if (FilterFactory == null)
            {
                throw new ArgumentNullException(nameof(FilterFactory));
            }
            
            this.FilterFactorys.Add(FilterFactory);
        }

        public TFilterGroup CreateFilterGroup()
        {
            List<TFilter> Filters = new List<TFilter>();
            int count = this.FilterFactorys.Count;
            for (int i = 0; i < count; i++)
            {
                var Filter = this.FilterFactorys[i]();
                if (Filter == null)
                {
                    throw new NullReferenceException(nameof(Filter));
                }
                
                Filters.Add(Filter);
            }

            return this.FilterGroupCreator(Filters);
        }

        public void Dispose()
        {
            this.FilterGroupCreator = null;
            
            if (this.FilterFactorys != null)
            {
                this.FilterFactorys.Clear();
                this.FilterFactorys = null;
            }
        }
    }
}