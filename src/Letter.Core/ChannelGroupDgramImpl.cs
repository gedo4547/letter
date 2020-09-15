using System.Buffers;
using System.Collections.Generic;

namespace Letter
{
    public class ChannelGroupDgramImpl<TContext, TChannel> : AChannelGroup<TChannel, TContext>
        where TContext : IContext
        where TChannel : IDgramChannel<TContext>
    {
        public ChannelGroupDgramImpl(List<TChannel> channels) : base(channels)
        {
        }

        public void OnChannelRead(TContext context, ref WrappedDgramReader reader)
        {
            ChannelArgs args = new ChannelArgs();
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelRead(context, ref reader, ref args);
            }
            reader.Flush();
        }

        public void OnChannelWrite(TContext context, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            ChannelArgs args = new ChannelArgs();
            args.buffer = sequence;
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelWrite(context, ref writer, ref args);
            }
            writer.Flush();
        }

        public void OnChannelWrite(TContext context, ref WrappedDgramWriter writer, object obj)
        {
            ChannelArgs args = new ChannelArgs();
            args.item = obj;
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelWrite(context, ref writer, ref args);
            }
            
            writer.Flush();
        }
    }
}