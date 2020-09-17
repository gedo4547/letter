using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class AChannelFilterGroupFactory<TSession, TFilter, TFilterGroup> : IChannelFilterGroupFactory<TSession, TFilter, TFilterGroup>
        where TSession : ISession
        where TFilter : IChannelFilter<TSession>
        where TFilterGroup : IChannelFilterGroup<TSession, TFilter>
    {
        private List<Func<TFilter>> filterFactorys = new List<Func<TFilter>>();
        
        public void AddFilterFactory(Func<TFilter> filterFactory)
        {
            if (filterFactory == null)
            {
                throw new  ArgumentNullException(nameof(filterFactory));
            }
            
            this.filterFactorys.Add(filterFactory);
        }

        public TFilterGroup CreateFilterGroup()
        {
            List<TFilter> Filters = new List<TFilter>();
            foreach (var FilterFactory in filterFactorys)
            {
                var Filter = FilterFactory();
                if (Filter == null)
                    throw new NullReferenceException(nameof(Filter));
                
                Filters.Add(Filter);
            }

            return this.FilterGroupCreator(Filters);
        }

        protected abstract TFilterGroup FilterGroupCreator(List<TFilter> Filters);
        

        public ValueTask DisposeAsync()
        {
            if (this.filterFactorys != null)
            {
                this.filterFactorys.Clear();
                this.filterFactorys = null;
            }

            return default;
        }
    }
}