using System;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ADgramBootstrap<TOptions, TSession, TFilter, TChannel> : ABootstrap<TOptions, TChannel>, IDgramBootstrap<TOptions, TSession, TFilter, TChannel>
        where TOptions : class, IOptions, new()
        where TSession : ISession
        where TFilter : IDgramChannelFilter<TSession>
        where TChannel : IChannel
    {
        private DgramChannelFilterGroupFactory<TSession, TFilter> groupFactory = new DgramChannelFilterGroupFactory<TSession, TFilter>();
        
        public override Task<TChannel> BuildAsync()
        {
            base.BuildAsync();
            
            return this.ChannelFactory(this.options, this.groupFactory);
        }

        protected abstract Task<TChannel> ChannelFactory(TOptions options, DgramChannelFilterGroupFactory<TSession, TFilter> groupFactory);
        
        public void AddChannelFilter<TChannelFilter>() where TChannelFilter : TFilter, new()
        {
            this.AddChannelFilter(() => { return new TChannelFilter(); });
        }

        public void AddChannelFilter(Func<TFilter> filterFactory)
        {
            if (filterFactory == null)
            {
                throw new ArgumentNullException(nameof(filterFactory));
            }
            
            this.groupFactory.AddFilterFactory(filterFactory);
        }

        public override async ValueTask DisposeAsync()
        {
            if (this.groupFactory != null)
            {
                await this.groupFactory.DisposeAsync();
                this.groupFactory = null;
            }

            await base.DisposeAsync();
        }
    }
}