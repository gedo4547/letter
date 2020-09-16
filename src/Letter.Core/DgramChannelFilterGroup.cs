using System.Buffers;
using System.Collections.Generic;

namespace Letter
{
    
    public class DgramChannelFilterGroup<TSession, TFilter> : AChannelFilterGroup<TSession, TFilter>, IDgramChannelFilterGroup<TSession, TFilter>
        where TSession : ISession
        where TFilter : IDgramChannelFilter<TSession>
    {
        public DgramChannelFilterGroup(List<TFilter> filters) : base(filters)
        {
        }

        public void OnFilterRead(TSession session, ref WrappedDgramReader reader)
        {
            FilterArgs args = new FilterArgs();
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelRead(session, ref reader, ref args);
            }
            reader.Flush();
        }

        public void OnFilterWrite(TSession session, ref WrappedDgramWriter writer, object obj)
        {
            FilterArgs args = new FilterArgs();
            args.item = obj;
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelWrite(session, ref writer, ref args);
            }
            
            writer.Flush();
        }

        public void OnFilterWrite(TSession session, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> buffer)
        {
            FilterArgs args = new FilterArgs();
            args.buffer = buffer;
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelWrite(session, ref writer, ref args);
            }
            writer.Flush();
        }
    }
}