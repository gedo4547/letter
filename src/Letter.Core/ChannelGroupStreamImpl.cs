using System.Buffers;
using System.Collections.Generic;
using Letter.IO;

namespace Letter
{
    public class ChannelGroupStreamImpl<TContext> : AChannelGroup<IStreamChannel<TContext>, TContext, WrappedStreamReader, WrappedStreamWriter>
        where TContext : class, IContext
    {
        public ChannelGroupStreamImpl(List<IStreamChannel<TContext>> channels) : base(channels)
        {
        }

        public override void OnChannelRead(TContext context, ref WrappedStreamReader reader)
        {
            EventArgs args = new EventArgs();
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelRead(context, ref reader, ref args);
            }
            reader.Flush();
        }

        public override void OnChannelWrite(TContext context, ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> sequence)
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

        public override void OnChannelWrite(TContext context, ref WrappedStreamWriter writer, object obj)
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