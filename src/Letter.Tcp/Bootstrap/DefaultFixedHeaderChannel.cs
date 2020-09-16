using System;
using Letter.IO;

namespace Letter.Tcp
{
    public class DefaultFixedHeaderFilter : ITcpFilter
    {
        private const int PackHeaderBytesLen = 4;
        
        public DefaultFixedHeaderFilter(int maxLength = 4096)
        {
            this.maxLength = maxLength == 0 ? 4096 : maxLength;
        }
        
        private int maxLength;
        private PackPart currentReadPart = PackPart.Head;
        private int currentReadLength = PackHeaderBytesLen;

        public void OnFilterActive(ITcpContext context)
        {
            
        }

        public void OnFilterInactive(ITcpContext context)
        {
            
        }

        public void OnFilterException(ITcpContext context, Exception ex)
        {
            
        }

        public void OnFilterRead(ITcpContext context, ref WrappedStreamReader reader, ref EventArgs args)
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

        public void OnFilterWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
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