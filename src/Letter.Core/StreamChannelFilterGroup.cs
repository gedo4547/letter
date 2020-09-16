using System.Buffers;
using System.Collections.Generic;
using System.Net;

namespace Letter
{
    public class StreamChannelFilterGroup<TSession, TFilter> : AChannelFilterGroup<TSession, TFilter>, IStreamChannelFilterGroup<TSession, TFilter>
        where TSession : ISession
        where TFilter : IStreamChannelFilter<TSession>
    {
        public StreamChannelFilterGroup(List<TFilter> filters) : base(filters)
        {
        }

        public void OnFilterRead(TSession session, EndPoint remoteAddress, ref WrappedStreamReader reader)
        {
            ChannelArgs args = new ChannelArgs();
            
            int count = this.filters.Count;
            for (int i = 0; i < count; ++i)
            {
                this.filters[i].OnChannelRead(session, remoteAddress, ref reader, ref args);
            }
            reader.Flush();
        }

        public void OnFilterWrite(TSession session, EndPoint remoteAddress, ref WrappedStreamWriter writer, object obj)
        {
            ChannelArgs args = new ChannelArgs()
            {
                item = obj
            };

            int count = this.filters.Count;
            for (int i = 0; i < count; ++i)
            {
                this.filters[i].OnChannelWrite(session, remoteAddress, ref writer, ref args);
            }
            
            writer.Flush();
        }

        public void OnFilterWrite(TSession session, EndPoint remoteAddress, ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> buffer)
        {
            ChannelArgs args = new ChannelArgs()
            {
                buffer = buffer
            };

            int count = this.filters.Count;
            for (int i = 0; i < count; ++i)
            {
                this.filters[i].OnChannelWrite(session, remoteAddress, ref writer, ref args);
            }
            
            writer.Flush();
        }
    }
}