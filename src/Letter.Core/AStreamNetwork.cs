using System;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class AStreamNetwork<TSession, TChannel> : IStreamNetwork<TSession, TChannel>
        where TSession : ISession
        where TChannel : IStreamChannel<TSession>
    {
        private StreamChannelGroupFactory<TSession, TChannel> groupFactory = new StreamChannelGroupFactory<TSession, TChannel>();
        
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