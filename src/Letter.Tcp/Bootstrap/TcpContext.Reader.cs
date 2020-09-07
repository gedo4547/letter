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
        private PipeReader Input
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.client.Transport.Input; }
        }

        private async Task ReaderMemoryPolledIOAsync()
        {
            while (true)
            {
                ReadResult result = await this.Input.ReadAsync();
                if (result.IsCanceled || result.IsCompleted)
                {
                    break;
                }
                
                try
                {
                    var buffer = result.Buffer;
                    this.ReaderMemoryIO(ref buffer);
                }
                catch (Exception e)
                {
                    // this.listener.OnException(e);
                    break;
                }
            }
        
            this.Input.Complete();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReaderMemoryIO(ref ReadOnlySequence<byte> buffer)
        {
            // var reader = new WrappedStreamReader(
            //     this.Input,
            //     ref buffer,
            //     ref this.order);
            //
            // this.listener.OnChannelRead(null, ref reader);
        }
    }
}