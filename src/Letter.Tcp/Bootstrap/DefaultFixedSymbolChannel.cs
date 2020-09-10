using System;
using System.Buffers;
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

        public void OnTransportActive(ITcpContext context)
        {
            
        }

        public void OnTransportInactive(ITcpContext context)
        {
            
        }

        public void OnTransportException(ITcpContext context, Exception ex)
        {
            
        }

        public void OnTransportRead(ITcpContext context, ref WrappedStreamReader reader, ref EventArgs args)
        {
            while (reader.TryFindPosition(this.symbol, out SequencePosition endPosition))
            {
                args.buffer = reader.ReadRange(endPosition);
            }
        }

        public void OnTransportWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
        {
            writer.Write(ref args.buffer);
            writer.Write(this.symbol);
        }
    }
}