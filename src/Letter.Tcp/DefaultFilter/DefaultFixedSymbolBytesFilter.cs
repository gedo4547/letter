using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;

namespace Letter.Tcp
{
    public sealed class DefaultFixedSymbolBytesFilter : ITcpChannelFilter
    {
        public DefaultFixedSymbolBytesFilter() : this(new byte[] {(byte) '\n'})
        {
        }

        public DefaultFixedSymbolBytesFilter(byte[] symbol)
        {
            if(symbol == null)
                throw new ArgumentNullException(nameof(symbol));
            if(symbol.Length > 8)
                throw new Exception("Only symbols of length 8 are supported");

            this.symbol = symbol;
        }

        private byte[] symbol;
        private List<ReadOnlySequence<byte>> buffers = new List<ReadOnlySequence<byte>>();
        
        public void OnTransportActive(ITcpSession session) { }
        public void OnTransportInactive(ITcpSession session) { }
        public void OnTransportException(ITcpSession session, Exception ex) { }

        public void OnTransportRead(ITcpSession session, ref WrappedReader reader, List<Object> args)
        {
            this.buffers.Clear();
            args.Add(this.buffers);
            while (reader.TryFindPosition(this.symbol, out SequencePosition endPosition))
            {
                var buffer = reader.ReadBuffer(endPosition);
                var length = buffer.Length - this.symbol.Length;
                this.buffers.Add(buffer.Slice(buffer.Start, length));
            }
        }

        public void OnTransportWrite(ITcpSession session, ref WrappedWriter writer, List<Object> args)
        {
            var buffer = args[0] as byte[];
            writer.Write(buffer);
            writer.Write(this.symbol);
        }
    }
}