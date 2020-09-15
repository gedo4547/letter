using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public partial class UdpContext
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

        public UdpSocketReceiver ReceiverSocket
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.receiver; }
        }
        
        private async Task ReaderMemoryPolledIOAsync()
        {
            while (true)
            {
                var node = this.ReceiverPipeWriter.GetDgramNode();
                var memory = node.GetMomory();
                try
                {
                    Console.WriteLine("开始监听："+this.LoaclAddress+"                "+memory.Length);
                    int transportBytes = await this.ReceiverSocket.ReceiveAsync(this.LoaclAddress, memory);
                    Console.WriteLine("收到消息：" + transportBytes);
                    node.SettingPoint(this.ReceiverSocket.RemoteAddress);
                    node.SettingWriteLength(transportBytes);
                }
                catch (Exception ex)
                {
                    this.channelGroup.OnChannelException(this, ex);
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

                this.RemoteAddress = node.Point;
                Memory<byte> memory = node.GetReadableBuffer();
                var w_reader = new WrappedDgramReader(ref memory, ref this.Order);
                this.channelGroup.OnChannelRead(this, ref w_reader);
                node.ReleaseAsync().NoAwait();
            }
            
            this.ReceiverPipeReader.ReceiveAsync();
        }
    }
}