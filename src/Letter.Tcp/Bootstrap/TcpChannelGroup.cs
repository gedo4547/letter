using System.Buffers;
using System.Collections.Generic;
using Letter.IO;

namespace Letter.Tcp
{
    public class TcpChannelGroup : AChannelGroup<ITcpChannel, ITcpContext, WrappedStreamReader,  WrappedStreamWriter>
    {
        public TcpChannelGroup(List<ITcpChannel> channels) : base(channels)
        {
        }

        public override void OnChannelRead(ITcpContext context, ref WrappedStreamReader reader)
        {
            base.OnChannelRead(context, ref reader);
            reader.Flush();
        }

        public override void OnChannelWrite(ITcpContext context, ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            base.OnChannelWrite(context, ref writer, ref sequence);
            writer.Flush();
        }

        public override void OnChannelWrite(ITcpContext context, ref WrappedStreamWriter writer, object obj)
        {
            base.OnChannelWrite(context, ref writer, obj);
            writer.Flush();
        }
    }
}