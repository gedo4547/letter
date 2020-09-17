﻿using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public partial class UdpSession
    {
        public IDgramPipeReader SenderPipeReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.senderPipeline; }
        }

        public IDgramPipeWriter SenderPipeWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.senderPipeline; }
        }
        
        public UdpSocketSender SenderSocketSender
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.socketSender; }
        }
        
        public void StartReceiveSenderPipelineBuffer()
        {
            this.SenderPipeReader.ReceiveAsync();
        }
        
        private async void OnSenderPipelineReceiveBuffer(IDgramPipeReader reader)
        {
            while (true)
            {
                var node = reader.Read();
                if (node == null) break;

                var memory = node.GetReadableBuffer();
                var sequence = new ReadOnlySequence<byte>(memory);
                var address = node.Point;
                try
                {
                    await node.ReleaseAsync();
                    int transportBytes = await this.SenderSocketSender.SendAsync(address, sequence);
                }
                catch(Exception ex)
                {
                    if (SocketErrorHelper.IsSocketDisabledError(ex))
                    {
                        await this.DisposeAsync();
                    }
                    else
                    {
                        this.filterGroup.OnChannelException(this, ex);
                    }
                    return;
                }
            }
            
            this.SenderPipeReader.ReceiveAsync();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task WriteBufferAsync(EndPoint remotePoint, object obj)
        {
            var writer =  this.CreateDgramWriter(remotePoint);
            this.filterGroup.OnChannelWrite(this, ref writer, obj);
            
            return Task.CompletedTask;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task WriteBufferAsync(EndPoint remotePoint, ref ReadOnlySequence<byte> sequence)
        {
            var writer = this.CreateDgramWriter(remotePoint);
            this.filterGroup.OnChannelWrite(this, ref writer, ref sequence);
            
            return Task.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private WrappedDgramWriter CreateDgramWriter(EndPoint remotePoint)
        {
            this.SndAddress = remotePoint;
            var node = this.SenderPipeWriter.GetDgramNode();
            node.SettingPoint(remotePoint);
            
            return new WrappedDgramWriter(node, ref this.order, this.onMemoryWritePush);
        }

        private void OnMemoryWritePush(DgramMessageNode node)
        {
            this.SenderPipeWriter.Write(node);
        }
    }
}