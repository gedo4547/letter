using System;
using System.Buffers;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Letter.Udp
{
    partial class UdpSession
    {
        public IDgramPipelineReader SenderPipeReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.senderPipeline; }
        }

        public IDgramPipelineWriter SenderPipeWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.senderPipeline; }
        }
        
        public void StartReceiveSenderPipelineBuffer()
        {
            this.SenderPipeReader.ReceiveAsync();
        }
        
        private async void OnSenderPipelineReceiveBuffer(IDgramPipelineReader reader)
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
                    int transportBytes = await this.udpSocket.SendAsync(address, ref sequence);
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
            try
            {
                this.SenderPipeWriter.Write(node);
            }
            catch (Exception e)
            {
               this.filterGroup.OnChannelException(this, e);
            }
        }
    }
}