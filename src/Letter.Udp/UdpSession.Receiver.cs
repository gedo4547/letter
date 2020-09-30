using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Letter.Udp
{
    partial class UdpSession
    {
        public IDgramPipelineReader ReceiverPipeReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.receiverPipeline; }
        }
        
        public IDgramPipelineWriter ReceiverPipeWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.receiverPipeline; }
        }
        
        public void StartReceiveReceiverPipelineBuffer()
        {
            this.ReceiverPipeReader.ReceiveAsync();
        }
        
        private async Task SocketReceiveAsync()
        {
            while (true)
            {
                var node = this.ReceiverPipeWriter.GetDgramNode();
                var memory = node.GetMomory();
                try
                {
                    int transportBytes = await this.udpSocket.ReceiveAsync(this.LoaclAddress, ref memory);
                    node.SettingPoint(this.udpSocket.RemoteAddress);
                    node.SettingWriteLength(transportBytes);
                    this.ReceiverPipeWriter.Write(node);
                }
                catch(Exception ex)
                {
                    await node.ReleaseAsync();
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
        }

        private void OnReceiverPipelineReceiveBuffer(IDgramPipelineReader reader)
        {
            while (true)
            {
                DgramMessageNode node = reader.Read();
                if (node == null) break;

                this.RcvAddress = node.Point;
                Memory<byte> memory = node.GetReadableBuffer();
                var w_reader = new WrappedDgramReader(ref memory, ref this.order);
                this.filterGroup.OnChannelRead(this, ref w_reader);
                node.ReleaseAsync().NoAwait();
            }
            
            this.ReceiverPipeReader.ReceiveAsync();
        }
    }
}