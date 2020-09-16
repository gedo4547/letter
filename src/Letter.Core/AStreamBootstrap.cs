using System;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class AStreamBootstrap<TOptions, TSession, TFilter, TChannel> : ABootstrap<TOptions, TChannel>, IStreamBootstrap<TOptions, TSession, TFilter, TChannel>
        where TOptions : class, IOptions, new()
        where TSession : ISession
        where TFilter : IStreamChannelFilter<TSession>
        where TChannel : IChannel
    {
        private StreamChannelFilterGroupFactory<TSession, TFilter> groupFactory = new StreamChannelFilterGroupFactory<TSession, TFilter>();

        public override Task<TChannel> BuildAsync()
        {
            base.BuildAsync();

            return this.ChannelFactory(this.options, this.groupFactory.CreateFilterGroup());
        }

        protected abstract Task<TChannel> ChannelFactory(TOptions options, StreamChannelFilterGroup<TSession, TFilter> filterGroup);
        
        public void AddChannelFilter<T>() where T : TFilter, new()
        {
            this.AddChannelFilter(() => { return new T(); });
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