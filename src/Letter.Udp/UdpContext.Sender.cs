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
        public IUdpPipeReader SenderPipeReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.senderPipeline; }
        }
        
        public IUdpPipeWriter SenderPipeWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.senderPipeline; }
        }

        public UdpSocketSender SenderSocket
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.sender; }
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task SenderMemoryIOAsync(EndPoint remotePoint, object obj)
        {
            var writer = this.CreateDgramWriter(remotePoint);
            this.channelGroup.OnChannelWrite(this, ref writer, obj);
            
            return Task.CompletedTask;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task SenderMemoryIOAsync(EndPoint remotePoint, ref ReadOnlySequence<byte> sequence)
        {
            var writer = this.CreateDgramWriter(remotePoint);
            this.channelGroup.OnChannelWrite(this, ref writer, ref sequence);
            
            return Task.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private WrappedDgramWriter CreateDgramWriter(EndPoint remotePoint)
        {
            this.RemoteAddress = remotePoint;
            var node = this.SenderPipeWriter.GetDgramNode();
            node.SettingPoint(remotePoint);
            return new WrappedDgramWriter(node, ref this.Order, this.onMemoryPushEvent);
        }

        private void OnMemoryWritePush(UdpMessageNode node) => this.SenderPipeWriter.Write(node);

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
                    Console.WriteLine("发送发送发送发送发送发送发送发送"+node.Point);
                    await this.SenderSocket.SendAsync(node.Point, sequence);
                    await node.ReleaseAsync();
                }
                catch (Exception ex)
                {
                    this.channelGroup.OnChannelException(this, ex);
                    return;
                }
            }
            
            this.SenderPipeReader.ReceiveAsync();
        }
    }
}