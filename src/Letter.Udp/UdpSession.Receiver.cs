using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public partial class UdpSession
    {
        public IUdpPipeReader ReceiverPipeReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.receiverPipeline; }
        }
        
        public IUdpPipeWriter ReceiverPipeWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.receiverPipeline; }
        }

        public UdpSocketReceiver ReceiverSocketReceiver
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.socketReceiver; }
        }

        public void ReceiverStartReceiveBuffer()
        {
            this.ReceiverPipeReader.ReceiveAsync();
        }
        
        private async Task ReaderMemoryPolledIOAsync()
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
                catch (Exception ex)
                {
                    this.filterGroup.OnChannelException(this, ex);
                    return;
                }

                this.ReceiverPipeWriter.Write(node);
            }
        }
        
        private void OnReceiverPipelineReceiveBuffer(IUdpPipeReader reader)
        {
            while (true)
            {
                UdpMessageNode node = reader.Read();
                if (node == null) break;
                
                Memory<byte> memory = node.GetReadableBuffer();
                var w_reader = new WrappedDgramReader(ref memory, ref this.order);
                this.filterGroup.OnChannelRead(this, node.Point, ref w_reader);
                node.ReleaseAsync().NoAwait();
            }
            
            this.ReceiverPipeReader.ReceiveAsync();
        }
    }
}