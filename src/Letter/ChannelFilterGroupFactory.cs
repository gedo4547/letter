using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter
{
    public sealed class ChannelFilterGroupFactory<TSession, TChannelFilter> : IAsyncDisposable
        where TSession : ISession
        where TChannelFilter : IChannelFilter<TSession>
    {
        private List<Func<TChannelFilter>> creators = new List<Func<TChannelFilter>>();
        
        public void AddFilterCreator(Func<TChannelFilter> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            
            this.creators.Add(func);
        }

        public ChannelFilterGroup<TSession, TChannelFilter> CreateGroup()
        {
            var filters = new List<TChannelFilter>();
            for (int i = 0; i < this.creators.Count; i++)
            {
                var filter = this.creators[i]();
                if (filter == null)
                {
                    throw new ArgumentNullException("The filter created is empty");
                }
                
                filters.Add(filter);
            }

            return new ChannelFilterGroup<TSession, TChannelFilter>(filters);
        }

        public ValueTask DisposeAsync()
        {
            this.creators.Clear();

            return default;
        }
    }
}