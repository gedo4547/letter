using System;
using System.Buffers;
using System.Collections.Generic;
using Letter.IO;

namespace Letter.Tcp
{
    public partial class TcpContext
    {
        private List<ITcpChannel> Channels
        {
            get { return this.channels; }
        }
        
        private void OnTransportActive()
        {
            foreach (var channel in this.Channels)
            {
                channel.OnTransportActive(this);
            }
        }

        private void OnTransportInactive()
        {
            foreach (var channel in this.Channels)
            {
                channel.OnTransportInactive(this);
            }
        }

        private void OnTransportException(Exception ex)
        {
            foreach (var channel in this.Channels)
            {
                channel.OnTransportException(this, ex);
            }
        }

        private void OnTransportRead(ref WrappedStreamReader reader)
        {
            EventArgs args = new EventArgs();
            foreach (var channel in this.Channels)
            {
                channel.OnTransportRead(this, ref reader, ref args);
            }
            
            reader.Flush();
        }

        private void OnTransportWrite(ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            EventArgs args = new EventArgs();
            args.buffer = sequence;
            
            foreach (var channel in this.Channels)
            {
                channel.OnTransportWrite(this, ref writer, ref args);
            }
            
            writer.Flush();
        }
        
        
        private void OnTransportWrite(ref WrappedStreamWriter writer, object obj)
        {
            EventArgs args = new EventArgs();
            args.item = obj;
            
            foreach (var channel in this.Channels)
            {
                channel.OnTransportWrite(this, ref writer, ref args);
            }
            
            writer.Flush();
        }
    }
}