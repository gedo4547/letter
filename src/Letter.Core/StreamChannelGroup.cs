using System.Buffers;
using System.Collections.Generic;

namespace Letter.Box.ssss
{
    public class StreamChannelGroup<TSession, TChannel> : AChannelGroup<TSession, TChannel>, IStreamChannelGroup<TSession, TChannel>
        where TSession : ISession
        where TChannel : IStreamChannel<TSession>
    {
        public StreamChannelGroup(List<TChannel> channels) : base(channels)
        {
        }

        public void OnChannelRead(TSession session, ref WrappedStreamReader reader)
        {
            ChannelArgs args = new ChannelArgs();
            
            int count = this.channels.Count;
            for (int i = 0; i < count; ++i)
            {
                this.channels[i].OnChannelRead(session, ref reader, ref args);
            }
            reader.Flush();
        }

        public void OnChannelWrite(TSession session, ref WrappedStreamWriter writer, object obj)
        {
            ChannelArgs args = new ChannelArgs()
            {
                item = obj
            };

            int count = this.channels.Count;
            for (int i = 0; i < count; ++i)
            {
                this.channels[i].OnChannelWrite(session, ref writer, ref args);
            }
            
            writer.Flush();
        }

        public void OnChannelWrite(TSession session, ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> buffer)
        {
            ChannelArgs args = new ChannelArgs()
            {
                buffer = buffer
            };

            int count = this.channels.Count;
            for (int i = 0; i < count; ++i)
            {
                this.channels[i].OnChannelWrite(session, ref writer, ref args);
            }
            
            writer.Flush();
        }
    }
}