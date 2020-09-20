using System;

namespace Letter.Tcp
{
    public sealed class DefaultFixedHeaderChannelFilter : ITcpChannelFilter
    {
        private const int PackHeaderBytesLen = 4;

        public DefaultFixedHeaderChannelFilter() : this(4096)
        {
        }

        public DefaultFixedHeaderChannelFilter(int maxPackLength)
        {
            this.maxPackLength = maxPackLength < 1024 ? 1024 : maxPackLength;
        }

        private int maxPackLength;
        private PackPart currentReadPart = PackPart.Head;
        private int currentReadLength = PackHeaderBytesLen;


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
            while (true)
            {
                if (!reader.IsLengthEnough(this.currentReadLength))
                {
                    break;
                }
                
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
                    args.buffers.Add(reader.ReadRange(this.currentReadLength));
                    this.currentReadLength = PackHeaderBytesLen;
                    this.currentReadPart = PackPart.Head;
                }
            }
        }

        public void OnChannelWrite(ITcpSession session, ref WrappedStreamWriter writer, ref ChannelArgs args)
        {
            var buffer = args.buffers[0];
            if (buffer.Length > this.maxPackLength)
            {
                throw new Exception("pack length error！！！" + buffer.Length);
            }

            writer.Write((int)buffer.Length);
            writer.Write(ref buffer);
        }

        enum PackPart : byte
        {
            Head,
            Body,
        }
    }
}
