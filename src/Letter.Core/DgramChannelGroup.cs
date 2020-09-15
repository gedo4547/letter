using System.Buffers;
using System.Collections.Generic;

namespace Letter
{
    
    public class DgramChannelGroup<TSession, TChannel> : AChannelGroup<TSession, TChannel>, IDgramChannelGroup<TSession, TChannel>
        where TSession : ISession
        where TChannel : IDgramChannel<TSession>
    {
        public DgramChannelGroup(List<TChannel> channels) : base(channels)
        {
        }

        public void OnChannelRead(TSession session, ref WrappedDgramReader reader)
        {
            ChannelArgs args = new ChannelArgs();
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelRead(session, ref reader, ref args);
            }
            reader.Flush();
        }

        public void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, object obj)
        {
            ChannelArgs args = new ChannelArgs();
            args.item = obj;
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelWrite(session, ref writer, ref args);
            }
            
            writer.Flush();
        }

        public void OnChannelWrite(TSession session, ref WrappedDgramWriter writer, ref ReadOnlySequence<byte> buffer)
        {
            ChannelArgs args = new ChannelArgs();
            args.buffer = buffer;
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelWrite(session, ref writer, ref args);
            }
            writer.Flush();
        }
    }
}