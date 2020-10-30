using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;

namespace Letter.Tcp
{
    public sealed class DefaultFixedHeaderBytesFilter : ITcpChannelFilter
    {
        private const int PackHeaderBytesLen = 4;

        public DefaultFixedHeaderBytesFilter() : this(4096) { }
        public DefaultFixedHeaderBytesFilter(int maxPackLength)
        {
            this.maxPackLength = maxPackLength < 1024 ? 1024 : maxPackLength;
        }

        private int maxPackLength;
        private PackPart currentReadPart = PackPart.Head;
        private int currentReadLength = PackHeaderBytesLen;
        private List<ReadOnlySequence<byte>> buffers = new List<ReadOnlySequence<byte>>();


        public void OnTransportActive(ITcpSession session) { }
        public void OnTransportInactive(ITcpSession session) { }
        public void OnTransportException(ITcpSession session, Exception ex) { }

        public void OnTransportRead(ITcpSession session, ref WrappedReader reader, List<Object> args)
        {
            this.buffers.Clear();
            args.Add(this.buffers);
            while (reader.IsLengthEnough(this.currentReadLength))
            {
                if (this.currentReadPart == PackPart.Head)
                {
                    this.currentReadLength = reader.ReadInt32();
                    if (this.currentReadLength > this.maxPackLength)
                    {
                        throw new Exception("pack length error！！！" + currentReadLength);
                    }
                    this.currentReadPart = PackPart.Body;
                }
                else if (this.currentReadPart == PackPart.Body)
                {
                    this.buffers.Add(reader.ReadBuffer(this.currentReadLength));
                    this.currentReadLength = PackHeaderBytesLen;
                    this.currentReadPart = PackPart.Head;
                }
            }
        }

        public void OnTransportWrite(ITcpSession session, ref WrappedWriter writer, List<Object> args)
        {
            var buffer = args[0] as byte[];
            if (buffer.Length > this.maxPackLength)
            {
                throw new Exception("pack length error！！！" + buffer.Length);
            }

            writer.Write((int)buffer.Length);
            writer.Write(buffer);
        }
        
        enum PackPart : byte
        {
            Head,
            Body,
        }
    }
}