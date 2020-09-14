using System.Buffers;
using System.Collections.Generic;
using Letter.IO;

namespace Letter
{
    public class ChannelGroupStreamImpl<TContext, TChannel> : AChannelGroup<TChannel, TContext>
        where TContext : IContext
        where TChannel : IStreamChannel<TContext>
    {
        public ChannelGroupStreamImpl(List<TChannel> channels) : base(channels)
        {
        }

        public void OnChannelRead(TContext context, ref WrappedStreamReader reader)
        {
            EventArgs args = new EventArgs();
            
            int count = this.channels.Count;
            for (int i = 0; i < count; ++i)
            {
                this.channels[i].OnChannelRead(context, ref reader, ref args);
            }
            reader.Flush();
        }

        public void OnChannelWrite(TContext context, ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            EventArgs args = new EventArgs()
            {
                buffer = sequence
            };

            int count = this.channels.Count;
            for (int i = 0; i < count; ++i)
            {
                this.channels[i].OnChannelWrite(context, ref writer, ref args);
            }
            
            writer.Flush();
        }

        public void OnChannelWrite(TContext context, ref WrappedStreamWriter writer, object obj)
        {
            EventArgs args = new EventArgs()
            {
                item = obj
            };

            int count = this.channels.Count;
            for (int i = 0; i < count; ++i)
            {
                this.channels[i].OnChannelWrite(context, ref writer, ref args);
            }
            
            writer.Flush();
        }
    }
}