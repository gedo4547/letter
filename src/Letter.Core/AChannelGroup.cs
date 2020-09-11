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

        protected List<TChannel> channels;

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

        public abstract void OnChannelRead(TContext context, ref TReader reader);

        public abstract void OnChannelWrite(TContext context, ref TWriter writer, ref ReadOnlySequence<byte> sequence);

        public abstract void OnChannelWrite(TContext context, ref TWriter writer, object obj);

        public virtual void Dispose()
        {
            if (this.channels != null)
            {
                this.channels.Clear();
                this.channels = null;
            }
        }
    }
}