using System;
using System.Buffers;
using System.Collections.Generic;

namespace Letter
{
    public abstract class AChannelGroup<TChannel, TContext, TReader, TWriter> : IDisposable
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TChannel : IChannel<TContext, TReader, TWriter>
    {
        public AChannelGroup(List<TChannel> channels)
        {
            if (channels == null)
            {
                throw new ArgumentNullException(nameof(channels));
            }

            this.channels = channels;
        }

        private List<TChannel> channels;

        public void OnChannelActive(TContext context)
        {
            int count = channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelActive(context);
            }
        }

        public void OnChannelInactive(TContext context)
        {
            int count = channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelInactive(context);
            }
        }
        
        public void OnChannelException(TContext context, Exception ex)
        {
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelException(context, ex);
            }
        }
        
        public virtual void OnChannelRead(TContext context, ref TReader reader)
        {
            EventArgs args = new EventArgs();
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelRead(context, ref reader, ref args);
            }
        }
        
        public virtual void OnChannelWrite(TContext context, ref TWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            EventArgs args = new EventArgs();
            args.buffer = sequence;
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelWrite(context, ref writer, ref args);
            }
        }

        public virtual void OnChannelWrite(TContext context, ref TWriter writer, object obj)
        {
            EventArgs args = new EventArgs();
            args.item = obj;
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnChannelWrite(context, ref writer, ref args);
            }
        }

        public void Dispose()
        {
            if (this.channels != null)
            {
                this.channels.Clear();
                this.channels = null;
            }
        }
    }
}