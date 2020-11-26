using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using Letter.IO;

namespace Letter.Udp
{
    public sealed class DefaultBytesFilter : IUdpFilter
    {
        private List<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>();
        
        public void OnTransportActive(IUdpSession session)
        {
        }

        public void OnTransportInactive(IUdpSession session)
        {
        }

        public void OnTransportException(IUdpSession session, Exception ex)
        {
        }

        public void OnTransportRead(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            this.buffers.Clear();
            var buffer = reader.ReadBuffer((int)reader.Length);
            this.buffers.Add(buffer.First.GetBinaryArray());
            args.Value = this.buffers;
        }

        public void OnTransportWrite(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            byte[] bytes = args.Value as byte[];
            if (bytes != null)
            {
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
        }
    }
}