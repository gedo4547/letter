using System;
using Letter.IO;

namespace Letter.Tcp
{
    public class DefaultFixedSymbolChannel : ITcpChannel
    {
        public DefaultFixedSymbolChannel(byte[] symbol)
        {
            if (symbol.Length > 8)
            {
                throw new Exception("Only symbols of length 8 are supported");
            }
            
            this.symbol = symbol;
        }

        private byte[] symbol;

        public void OnChannelActive(ITcpContext context)
        {
            
        }

        public void OnChannelInactive(ITcpContext context)
        {
            
        }

        public void OnChannelException(ITcpContext context, Exception ex)
        {
            
        }

        public void OnChannelRead(ITcpContext context, ref WrappedStreamReader reader, ref EventArgs args)
        {
            while (reader.TryFindPosition(this.symbol, out SequencePosition endPosition))
            {
                var buffer = reader.ReadRange(endPosition);
                long length = buffer.Length - this.symbol.Length;
                
                args.buffer = buffer.Slice(0, length);
            }
        }

        public void OnChannelWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
        {
            writer.Write(ref args.buffer);
            writer.Write(this.symbol);
        }
    }
}