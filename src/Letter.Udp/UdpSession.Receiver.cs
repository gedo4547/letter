using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public partial class UdpSession
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

        public UdpSocketReceiver ReceiverSocketReceiver
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.socketReceiver; }
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
                    int transportBytes = await this.ReceiverSocketReceiver.ReceiveAsync(this.LoaclAddress, memory);
                    node.SettingPoint(this.ReceiverSocketReceiver.RemoteAddress);
                    node.SettingWriteLength(transportBytes);
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
                this.ReceiverPipeWriter.Write(node);
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