using System.Buffers;
using System.Collections.Generic;
using Letter.IO;

namespace Letter.Udp
{
    public class UdpChannelGroup : AChannelGroup<IUdpChannel, IUdpContext,WrappedDgramReader, WrappedDgramWriter >
    {
        public UdpChannelGroup(List<IUdpChannel> channels) : base(channels)
        {
        }

        public override void OnChannelRead(IUdpContext context, ref WrappedDgramReader reader)
        {
            base.OnChannelRead(context, ref reader);
            reader.Flush();
        }

        public override void OnChannelWrite(IUdpContext context, ref WrappedDgramWriter writer, object obj)
        {
            base.OnChannelWrite(context, ref writer, obj);
            writer.Flush();
        }

        public override void OnChannelWrite(IUdpContext context, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            base.OnChannelWrite(context, ref writer, ref sequence);
            writer.Flush();
        }
    }
}