using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Udp
{
    public partial class UdpContext
    {
        public IUdpPipeReader PipeReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.senderPipeline; }
        }
        
        public IUdpPipeWriter PipeWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.senderPipeline; }
        }

        public UdpSocketSender Sender
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.sender; }
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task SenderMemoryIOAsync(EndPoint remotePoint, ref ReadOnlySequence<byte> sequence)
        {
            var node = this.PipeWriter.GetDgramNode();
            node.SettingPoint(remotePoint);
            
            var writer = new WrappedDgramWriter(node, ref this.Order, this.onMemoryPushEvent);
            this.RemoteAddress = remotePoint;
            this.channelGroup.OnChannelWrite(this, ref writer, ref sequence);
            
            return Task.CompletedTask;
        }

        private void OnMemoryWritePush(UdpMessageNode node)
        {
            this.PipeWriter.Write(node);
        }
        
        private async void OnSenderPipelineReceiveBuffer(IUdpPipeReader reader)
        {
            while (true)
            {
                var node = reader.Read();
                if (node == null) break;

                var memory = node.GetReadableBuffer();
                var sequence = new ReadOnlySequence<byte>(memory);
                
                try
                {
                    int transportBytes = await this.Sender.SendAsync(node.Point, sequence);
                    await node.ReleaseAsync();
                }
                catch (Exception ex)
                {
                    // base.SocketExceptionInspect(ex);
                    return;
                }
            }
            
            this.PipeReader.ReceiveAsync();
        }
    }
}