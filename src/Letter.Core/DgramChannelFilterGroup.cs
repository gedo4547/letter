using System.Buffers;
using System.Collections.Generic;
using System.Net;

namespace Letter
{
    
    public class DgramChannelFilterGroup<TSession, TFilter> : AChannelFilterGroup<TSession, TFilter>, IDgramChannelFilterGroup<TSession, TFilter>
        where TSession : ISession
        where TFilter : IDgramChannelFilter<TSession>
    {
        public DgramChannelFilterGroup(List<TFilter> filters) : base(filters)
        {
        }

        public void OnChannelRead(TSession session, EndPoint remoteAddress, ref WrappedDgramReader reader)
        {
            ChannelArgs args = new ChannelArgs();
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelRead(session, remoteAddress, ref reader, ref args);
            }
            reader.Flush();
        }

        public void OnChannelWrite(TSession session, EndPoint remoteAddress, ref WrappedDgramWriter writer, object obj)
        {
            ChannelArgs args = new ChannelArgs();
            args.item = obj;
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelWrite(session, remoteAddress, ref writer, ref args);
            }
            
            writer.Flush();
        }

        public void OnChannelWrite(TSession session, EndPoint remoteAddress, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> buffer)
        {
            ChannelArgs args = new ChannelArgs();
            args.buffer = buffer;
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelWrite(session, remoteAddress, ref writer, ref args);
            }
            writer.Flush();
        }
    }
}