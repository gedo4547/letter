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
            try
            {
                int count = this.filters.Count;
                for (int i = 0; i < count; i++)
                {
                    this.filters[i].OnTransportActive(session);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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
                session.DisposeAsync().NoAwait();
            }
        }

        public void OnTransportRead(TSession session, ref WrappedReader reader)
        {
            try
            {
                this.readArgs.Value = null;
                int count = this.filters.Count;
                for (int i = 0; i < count; i++)
                {
                    this.filters[i].OnTransportRead(session, ref reader, this.readArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public void OnTransportWrite(TSession session, ref WrappedWriter writer, object o)
        {
            try
            {
                this.writeArgs.Value = o;
                int count = this.filters.Count;
                for (int i = 0; i < count; i++)
                {
                    this.filters[i].OnTransportWrite(session, ref writer, this.writeArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ValueTask DisposeAsync()
        {
            this.filters.Clear();

            return default;
        }
    }
}