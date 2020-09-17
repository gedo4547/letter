using System;
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
        
        public UdpSocketSender SenderSocketSender
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.socketSender; }
        }
        
        public void StartReceiveSenderPipelineBuffer()
        {
            this.SenderPipeReader.ReceiveAsync();
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
                    int transportBytes = await this.SenderSocketSender.SendAsync(node.Point, sequence);
                    await node.ReleaseAsync();
                    if (transportBytes == 0)
                    {
                        Console.WriteLine($"{name}---Sender----111111111111111111111>>");
                        this.CloseAsync().NoAwait();
                        return;
                    }
                }
                catch (SocketException ex) when (SocketErrorHelper.IsConnectionResetError(ex.SocketErrorCode))
                {
                    Console.WriteLine($"{name}---Sender----22222222222222222222222>>");
                    this.CloseAsync().NoAwait();
                    return;
                }
                catch (Exception ex) when ((ex is SocketException socketEx &&
                                            SocketErrorHelper.IsConnectionAbortError(socketEx.SocketErrorCode)) ||
                                           ex is ObjectDisposedException)
                {
                    Console.WriteLine($"{name}---Sender----333333333333333333333333333>>");
                    this.CloseAsync().NoAwait();
                    return;
                }
                catch (Exception ex)
                {
                    this.filterGroup.OnChannelException(this, ex);
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

        private void OnMemoryWritePush(UdpMessageNode node)
        {
            this.SenderPipeWriter.Write(node);
        }
    }
}