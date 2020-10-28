using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Letter
{
    public sealed class ChannelFilterGroup<TSession, TChannelFilter> : IAsyncDisposable
        where TSession : ISession
        where TChannelFilter : IChannelFilter<TSession>
    {
        public ChannelFilterGroup(List<TChannelFilter> filters)
        {
            this.filters = filters;
        }

        private object readArgs;
        private object writeArgs;
        private List<TChannelFilter> filters;
        
        public void OnChannelActive(TSession session)
        {
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelActive(session);
            }
        }

        public void OnChannelInactive(TSession session)
        {
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelInactive(session);
            }
        }

        public void OnChannelException(TSession session, Exception ex)
        {
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelException(session, ex);
            }
        }

        public void OnChannelRead(TSession session, ref WrappedReader reader)
        {
            this.readArgs = null;
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelRead(session, ref reader, this.readArgs);
            }
        }
        
        public void OnChannelWrite(TSession session, ref WrappedWriter writer)
        {
            this.writeArgs = null;
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelWrite(session, ref writer, this.writeArgs);
            }
        }

        public ValueTask DisposeAsync()
        {
            this.filters.Clear();

            return default;
        }
    }
}