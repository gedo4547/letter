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

        private List<ReadOnlySequence<byte>> readBuffers = new List<ReadOnlySequence<byte>>();
        private List<ReadOnlySequence<byte>> writeBuffers = new List<ReadOnlySequence<byte>>();

        public void OnChannelRead(TSession session, ref WrappedDgramReader reader)
        {
            this.readBuffers.Clear();
            ChannelArgs args = new ChannelArgs()
            {
                buffers = this.readBuffers
            };
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelRead(session, ref reader, ref args);
            }
            reader.Flush();
        }

        public void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, object obj)
        {
            this.writeBuffers.Clear();
            ChannelArgs args = new ChannelArgs();
            args.item = obj;
            args.buffers = this.writeBuffers;
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelWrite(session, ref writer, ref args);
            }
            
            writer.Flush();
        }

        public void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> buffer)
        {
            this.writeBuffers.Clear();
            this.writeBuffers.Add(buffer);
            
            ChannelArgs args = new ChannelArgs();
            args.buffers = this.writeBuffers;
            
            int count = this.filters.Count;
            for (int i = 0; i < count; i++)
            {
                this.filters[i].OnChannelWrite(session, ref writer, ref args);
            }
            writer.Flush();
        }
    }
}