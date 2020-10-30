using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;

namespace Letter
{
    public sealed class FilterPipeline<TSession> : IFilterPipeline<TSession>, IAsyncDisposable where TSession : ISession
    {
        private List<Object> readArgs = new List<object>();
        private List<Object> writeArgs = new List<object>();
        
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
                this.readArgs.Clear();
                int count = this.filters.Count;
                Console.WriteLine("filters.Count>>" + count);
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
        
        public void OnTransportWrite(TSession session, ref WrappedWriter writer, Object o)
        {
            try
            {
                this.writeArgs.Clear();
                this.writeArgs.Add(o);
                
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