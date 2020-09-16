using System;
using System.Buffers;
using System.Collections.Generic;

namespace Letter
{
    public abstract class AFilterGroup<TFilter, TContext, TReader, TWriter> : IDisposable
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TFilter : IFilter<TContext, TReader, TWriter>
    {
        public AFilterGroup(List<TFilter> Filters)
        {
            if (Filters == null)
            {
                throw new ArgumentNullException(nameof(Filters));
            }

            this.Filters = Filters;
        }

        private List<TFilter> Filters;

        public void OnFilterActive(TContext context)
        {
            int count = Filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.Filters[i].OnFilterActive(context);
            }
        }

        public void OnFilterInactive(TContext context)
        {
            int count = Filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.Filters[i].OnFilterInactive(context);
            }
        }
        
        public void OnFilterException(TContext context, Exception ex)
        {
            int count = this.Filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.Filters[i].OnFilterException(context, ex);
            }
        }
        
        public virtual void OnFilterRead(TContext context, ref TReader reader)
        {
            EventArgs args = new EventArgs();
            int count = this.Filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.Filters[i].OnFilterRead(context, ref reader, ref args);
            }
        }
        
        public virtual void OnFilterWrite(TContext context, ref TWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            EventArgs args = new EventArgs();
            args.buffer = sequence;
            
            int count = this.Filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.Filters[i].OnFilterWrite(context, ref writer, ref args);
            }
        }

        public virtual void OnFilterWrite(TContext context, ref TWriter writer, object obj)
        {
            EventArgs args = new EventArgs();
            args.item = obj;
            
            int count = this.Filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.Filters[i].OnFilterWrite(context, ref writer, ref args);
            }
        }

        public void Dispose()
        {
            if (this.Filters != null)
            {
                this.Filters.Clear();
                this.Filters = null;
            }
        }
    }
}