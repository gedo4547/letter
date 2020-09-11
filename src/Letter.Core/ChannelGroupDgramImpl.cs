using System.Buffers;
using System.Collections.Generic;
using Letter.IO;

namespace Letter
{
    public class ChannelGroupDgramImpl<TContext> : AChannelGroup<IDgramChannel<TContext>, TContext, WrappedDgramReader, WrappedDgramWriter>
        where TContext : class, IContext
    {
        public ChannelGroupDgramImpl(List<IDgramChannel<TContext>> channels) : base(channels)
        {
        }

        public override void OnChannelRead(TContext context, ref WrappedDgramReader reader)
        {
            EventArgs args = new EventArgs();
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelRead(context, ref reader, ref args);
            }
            reader.Flush();
        }

        public override void OnChannelWrite(TContext context, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            EventArgs args = new EventArgs();
            args.buffer = sequence;
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelWrite(context, ref writer, ref args);
            }
            writer.Flush();
        }

        public override void OnChannelWrite(TContext context, ref WrappedDgramWriter writer, object obj)
        {
            EventArgs args = new EventArgs();
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