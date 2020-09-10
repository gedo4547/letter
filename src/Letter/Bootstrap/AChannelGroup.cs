﻿using System;
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

        public void OnTransportActive(TContext context)
        {
            int count = channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnTransportActive(context);
            }
        }

        public void OnTransportInactive(TContext context)
        {
            int count = channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnTransportInactive(context);
            }
        }
        
        public void OnTransportException(TContext context, Exception ex)
        {
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                this.channels[i].OnTransportException(context, ex);
            }
        }
        
        public virtual void OnTransportRead(TContext context, ref TReader reader)
        {
            EventArgs args = new EventArgs();
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                var channel = this.channels[i];
                channel.OnTransportRead(context, ref reader, ref args);
            }
        }
        
        public virtual void OnTransportWrite(TContext context, ref TWriter writer, ref ReadOnlySequence<byte> sequence)
        {
            EventArgs args = new EventArgs();
            args.buffer = sequence;
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                var channel = this.channels[i];
                channel.OnTransportWrite(context, ref writer, ref args);
            }
        }

        public virtual void OnTransportWrite(TContext context, ref TWriter writer, object obj)
        {
            EventArgs args = new EventArgs();
            args.item = obj;
            
            int count = this.channels.Count;
            for (int i = 0; i < count; i++)
            {
                var channel = this.channels[i];
                channel.OnTransportWrite(context, ref writer, ref args);
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