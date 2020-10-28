using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Letter
{
    public sealed class FilterPipeline<TSession> : IFilterPipeline<TSession>, IAsyncDisposable where TSession : ISession
    {
        private object readArgs;
        private object writeArgs;
        
        private List<IFilter<TSession>> filters = new List<IFilter<TSession>>();
        
        public void Add(IFilter<TSession> filter)
        {
            this.filters.Add(filter);
        }
        
        public void OnChannelActive(TSession session)
        {
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnTransportActive(session);
            }
        }

        public void OnChannelInactive(TSession session)
        {
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnTransportInactive(session);
            }
        }

        public void OnChannelException(TSession session, Exception ex)
        {
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnTransportException(session, ex);
            }
        }

        public void OnChannelRead(TSession session, ref WrappedReader reader)
        {
            this.readArgs = null;
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnTransportRead(session, ref reader, this.readArgs);
            }
        }
        
        public void OnChannelWrite(TSession session, ref WrappedWriter writer)
        {
            this.writeArgs = null;
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnTransportWrite(session, ref writer, this.writeArgs);
            }
        }

        public ValueTask DisposeAsync()
        {
            this.filters.Clear();

            return default;
        }
    }
}