using System;

namespace Letter.Tcp
{
    public sealed class DefaultFixedSymbolChannelFilter : ITcpChannelFilter
    {
        public DefaultFixedSymbolChannelFilter() : this(new byte[] {(byte) '\n'})
        {
        }

        public DefaultFixedSymbolChannelFilter(byte[] symbol)
        {
            if(symbol == null)
                throw new ArgumentNullException(nameof(symbol));
            if(symbol.Length > 8)
                throw new Exception("Only symbols of length 8 are supported");

            this.symbol = symbol;
        }

        private byte[] symbol;

        public void OnChannelActive(ITcpSession session)
        {
        }

        public void OnChannelException(ITcpSession session, Exception ex)
        {
        }

        public void OnChannelInactive(ITcpSession session)
        {
        }

        public void OnChannelRead(ITcpSession session, ref WrappedStreamReader reader, ref ChannelArgs args)
        {
            while (reader.TryFindPosition(this.symbol, out SequencePosition endPosition))
            {
                var buffer = reader.ReadRange(endPosition);
                var length = buffer.Length - this.symbol.Length;
                args.buffers.Add(buffer.Slice(buffer.Start, length));
            }
        }

        public void OnChannelWrite(ITcpSession session, ref WrappedStreamWriter writer, ref ChannelArgs args)
        {
            var buffer = args.buffers[0];
            writer.Write(ref buffer);
            writer.Write(this.symbol);
        }
    }
}
