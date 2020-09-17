using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
                    
                    if (transportBytes == 0)
                    {
                        Console.WriteLine($"{name}---Receiver-----1111111111111111111111111111>>>>");
                        node.ReleaseAsync().NoAwait();
                        this.CloseAsync().NoAwait();
                        return;
                    }
                    
                    node.SettingPoint(this.ReceiverSocketReceiver.RemoteAddress);
                    node.SettingWriteLength(transportBytes);
                }
                catch (SocketException ex) when (SocketErrorHelper.IsConnectionResetError(ex.SocketErrorCode))
                {
                    Console.WriteLine($"{name}---Receiver-----222222222222222222222222>>>>");
                    this.CloseAsync().NoAwait();
                    return;
                }
                catch (Exception ex) when ((ex is SocketException socketEx &&
                                            SocketErrorHelper.IsConnectionAbortError(socketEx.SocketErrorCode)) ||
                                            ex is ObjectDisposedException)
                {
                    Console.WriteLine($"{name}---Receiver------333333333333333333333333333>>>>");
                    this.CloseAsync().NoAwait();
                    return;
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