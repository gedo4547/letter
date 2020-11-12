using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Letter.IO
{
    public sealed class FilterPipeline<TSession> : IFilterPipeline<TSession>, IAsyncDisposable where TSession : ISession
    {
        private WrappedArgs readArgs = new WrappedArgs();
        private WrappedArgs writeArgs = new WrappedArgs();
        private List<IFilter<TSession>> filters = new List<IFilter<TSession>>();
        
        public void Add(IFilter<TSession> filter)
        {
            this.filters.Add(filter);
        }
        
        public void OnTransportActive(TSession session)
        {
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnTransportActive(session);
            }
        }

        public void OnTransportInactive(TSession session)
        {
            try
            {
                int count = this.filters.Count;
                for (int i = 0; i < count; i++)
                {
                    this.filters[i].OnTransportInactive(session);
                }
            }
            catch
            {
                
            }
        }

        public void OnTransportException(TSession session, Exception ex)
        {
            try
            {
                int count = this.filters.Count;
                for (int i = 0; i < count; i++)
                {
                    this.filters[i].OnTransportException(session, ex);
                }
            }
            catch
            {
                
            }
        }

        public void OnTransportRead(TSession session, ref WrappedReader reader)
        {
            this.readArgs.Value = null;
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnTransportRead(session, ref reader, this.readArgs);
            }
        }
        
        public void OnTransportWrite(TSession session, ref WrappedWriter writer, object o)
        {
            this.writeArgs.Value = o;
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