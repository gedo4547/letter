using System;
using System.IO.Pipelines;
using System.Net.Sockets;
using Letter;

namespace UdpPipline
{
    public class UdpChannel
    {
        public UdpChannel()
        {
            this.socket = new UdpSocket(new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp), PipeScheduler.Inline);
        }

        private UdpSocket socket;

        public void Receive()
        {
            
            // this.socket.ReceiveAsync(this.socket.LocalAddress, )
        }





    }
}