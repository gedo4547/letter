﻿using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    public partial class TcpContext
    {
        private PipeWriter Output
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.client.Transport.Output; }
        }
        
        private object syncLock = new object();
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task SenderMemoryIOAsync(object obj)
        {
            try
            {
                lock (syncLock)
                {
                    WrappedStreamWriter writer = new WrappedStreamWriter(this.Output, ref this.order);
                    this.FilterGroup.OnFilterWrite(this, ref writer, obj);
                }
            }
            catch (Exception e)
            {
                this.FilterGroup.OnFilterException(this, e);
                return Task.CompletedTask;
            }

            return this.WriteFlushAsync();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task SenderMemoryIOAsync(ref ReadOnlySequence<byte> sequence)
        {
            try
            {
                lock (syncLock)
                {
                    WrappedStreamWriter writer = new WrappedStreamWriter(this.Output, ref this.order);
                    this.FilterGroup.OnFilterWrite(this, ref writer, ref sequence);
                }
            }
            catch (Exception e)
            {
                this.FilterGroup.OnFilterException(this, e);
                return Task.CompletedTask;
            }

            return this.WriteFlushAsync();
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task WriteFlushAsync()
        {
            FlushResult result = await this.Output.FlushAsync();
            if (result.IsCompleted || result.IsCanceled)
            {
                this.Output.Complete();
            }
        }
    }
}