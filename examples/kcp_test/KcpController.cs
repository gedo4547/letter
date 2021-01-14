using System;
using System.IO.Pipelines;
using System.Net;
using System.Collections.Generic;

using Letter.IO;
using Letter.Kcp;
using Letter.Udp;

namespace kcp_test
{
    class KcpController : AKcpController
    {
        private const int c_message = 1;
        private const int k_message = 2;


        private Dictionary<uint, IKcpSession> sessions = new Dictionary<uint, IKcpSession>();

        public void Connect(uint conv, EndPoint remoteAddress)
        {
            if(this.sessions.ContainsKey(conv))
            {
                Console.WriteLine("已经存在的频道");
                return;
            }

            var session = base.Create(conv, remoteAddress);
            this.sessions.Add(session.CurrentConv, session);
        }

        public override void OnUdpException(IUdpSession session, Exception ex)
        {
            foreach (var item in this.sessions)
            {
                base.SendExceptionTo(item.Value, ex);
            }
        }

        public override void OnUdpInput(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            //消息类型
            var messageType = reader.ReadInt32();

            var buffer = reader.ReadBuffer((int)reader.Length);
            
            //频道号
            var convSpan = buffer.Slice(buffer.Start, 4).First.Span;
            var conv = OrderOperators.ReadUInt32(convSpan);
            
            if (!this.sessions.ContainsKey(conv)) return;
            Logger.Info(">>>>>>>>>>>>conv>"+conv+"  messageType>"+messageType);
            var kcpSession = this.sessions[conv];
            if(messageType == k_message)
            {
                base.SendKcpMessageTo(kcpSession, ref buffer);
            }
            else if(messageType == c_message)
            {
                base.SendUdpMessageTo(kcpSession, ref buffer);
            }
        }

        public override void OnUdpOutput(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            switch(args.Value)
            {
                //普通udp协议
                case byte[] udpPack:
                    writer.Write(c_message);
                    writer.Write(udpPack);
                    break;

                //kcp协议
                case IWrappedMemory wrappedMemory:
                    writer.Write(k_message);
                    writer.Write(wrappedMemory.GetReadableMemory());
                    break;
            }
        }

        public override void OnSessionClosed(IKcpSession session)
        {
            if(!this.sessions.ContainsKey(session.CurrentConv))
            {
                return;
            }

            this.sessions.Remove(session.CurrentConv);
        }

        public override void Dispose()
        {
            base.Dispose();

            this.sessions.Clear();
        }
    }
}