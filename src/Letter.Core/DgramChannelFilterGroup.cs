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

        public void OnChannelRead(TSession session, ref WrappedDgramReader reader)
        {
            ChannelArgs args = new ChannelArgs();
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelRead(session, ref reader, ref args);
            }
            reader.Flush();
        }

        public void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, object obj)
        {
            ChannelArgs args = new ChannelArgs();
            args.item = obj;
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelWrite(session, ref writer, ref args);
            }
            
            writer.Flush();
        }

        public void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> buffer)
        {
            ChannelArgs args = new ChannelArgs();
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