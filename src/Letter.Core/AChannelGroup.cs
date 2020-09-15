using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter
{
    public class AChannelGroup<TSession, TChannel> : IChannelGroup<TSession, TChannel>
        where TSession : ISession
        where TChannel : IChannel<TSession>
    {
        public AChannelGroup(List<TChannel> channels)
        {
            this.channels = channels;
        }

        protected List<TChannel> channels;
        
        public void OnChannelActive(TSession session)
        {
            int count = channels.Count;
            for (int i = 0; i < count; ++i)
            {
                this.channels[i].OnChannelActive(session);
            }
        }

        public void OnChannelInactive(TSession session)
        {
            int count = channels.Count;
            for (int i = 0; i < count; ++i)
            {
                this.channels[i].OnChannelInactive(session);
            }
        }

        public void OnChannelException(TSession session, Exception ex)
        {
            int count = this.channels.Count;
            for (int i = 0; i < count; ++i)
            {
                this.channels[i].OnChannelException(session, ex);
            }
        }
        
        public ValueTask DisposeAsync()
        {
            if (this.channels != null)
            {
                this.channels.Clear();
                this.channels = null;
            }

            return default;
        }
    }
}