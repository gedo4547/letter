using System;
using Letter.IO;

namespace Letter.Tcp
{
    public class DefaultFixedHeaderChannel : ITcpChannel
    {
        private const int PackHeaderBytesLen = 4;
        
        public DefaultFixedHeaderChannel(int maxLength = 4096)
        {
            this.maxLength = maxLength == 0 ? 4096 : maxLength;
        }
        
        private int maxLength;
        private PackPart currentReadPart = PackPart.Head;
        private int currentReadLength = PackHeaderBytesLen;

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
            while (true)
            {
                if (!reader.IsLengthEnough(this.currentReadLength))
                {
                    break;
                }
                
                if (this.currentReadPart == PackPart.Head)
                {
                    this.currentReadLength = reader.ReadInt32();
                    if (this.currentReadLength > this.maxLength)
                    {
                        throw new Exception("error pack length！！！" + currentReadLength);
                    }
                    this.currentReadPart = PackPart.Body;
                }
                else if (this.currentReadPart == PackPart.Body)
                {
                    args.buffer= reader.ReadRange(this.currentReadLength);
                    this.currentReadLength = PackHeaderBytesLen;
                    this.currentReadPart = PackPart.Head;
                }
            }

           
        }

        public void OnChannelWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
        {
            if (args.buffer.Length > this.maxLength)
            {
                throw new Exception("pack length error！！！" + args.buffer.Length);
            }
            
            writer.Write((int)args.buffer.Length);
            writer.Write(ref args.buffer);
        }
        
        enum PackPart : byte
        {
            Head,
            Body,
        }
    }
}