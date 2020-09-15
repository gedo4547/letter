using System;
using System.Threading.Tasks;

namespace Letter.Box.ssss
{
    public abstract class ADgramNetwork<TSession, TChannel>  : IDgramNetwork<TSession, TChannel>
        where TSession : ISession
        where TChannel : IDgramChannel<TSession>
    {
        private DgramChannelGroupFactory<TSession, TChannel> groupFactory = new DgramChannelGroupFactory<TSession, TChannel>();
        
        public void AddChannel(Func<TChannel> channelFactory)
        {
            this.groupFactory.AddChannelFactory(channelFactory);
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (this.groupFactory != null)
            {
                await this.groupFactory.DisposeAsync();
                this.groupFactory = null;
            }
        }
    }
}