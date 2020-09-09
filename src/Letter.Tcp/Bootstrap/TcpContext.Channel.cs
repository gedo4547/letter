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
            int length = this.Channels.Count;
            for (int i = 0; i < length; i++)
            {
                var channel = this.Channels[i];
                channel.OnTransportActive(this);
            }
        }

        private void OnTransportInactive()
        {
            int length = this.Channels.Count;
            for (int i = 0; i < length; i++)
            {
                var channel = this.Channels[i];
                channel.OnTransportInactive(this);
            }
        }

        private void OnTransportException(Exception ex)
        {
            int length = this.Channels.Count;
            for (int i = 0; i < length; i++)
            {
                var channel = this.Channels[i];
                channel.OnTransportException(this, ex);
            }
        }

        private void OnTransportRead(ref WrappedStreamReader reader)
        {
            EventArgs args = new EventArgs();
            int length = this.Channels.Count;
            for (int i = 0; i < length; i++)
            {
                var channel = this.Channels[i];
                channel.OnTransportRead(this, ref reader, ref args);
            }
            
            reader.Flush();
        }

        private void OnTransportWrite(ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            EventArgs args = new EventArgs();
            args.buffer = sequence;
            
            int length = this.Channels.Count;
            for (int i = 0; i < length; i++)
            {
                var channel = this.Channels[i];
                channel.OnTransportWrite(this, ref writer, ref args);
            }
            
            writer.Flush();
        }
        
        
        private void OnTransportWrite(ref WrappedStreamWriter writer, object obj)
        {
            EventArgs args = new EventArgs();
            args.item = obj;
            
            int length = this.Channels.Count;
            for (int i = 0; i < length; i++)
            {
                var channel = this.Channels[i];
                channel.OnTransportWrite(this, ref writer, ref args);
            }
            
            writer.Flush();
        }
    }
}