using System.Buffers;
using System.Collections.Generic;
using Letter.IO;

namespace Letter.Tcp
{
    public class TcpFilterGroup : AFilterGroup<ITcpFilter, ITcpContext, WrappedStreamReader,  WrappedStreamWriter>
    {
        public TcpFilterGroup(List<ITcpFilter> Filters) : base(Filters)
        {
        }

        public override void OnFilterRead(ITcpContext context, ref WrappedStreamReader reader)
        {
            base.OnFilterRead(context, ref reader);
            reader.Flush();
        }

        public override void OnFilterWrite(ITcpContext context, ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            base.OnFilterWrite(context, ref writer, ref sequence);
            writer.Flush();
        }

        public override void OnFilterWrite(ITcpContext context, ref WrappedStreamWriter writer, object obj)
        {
            base.OnFilterWrite(context, ref writer, obj);
            writer.Flush();
        }
    }
}