﻿using System;
using Letter;
using Letter.Tcp;

namespace tcp_test1
{
    public class TcpTestFilter_Server : ITcpChannelFilter
    {
        public void OnChannelActive(ITcpSession session)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Server)}.{nameof(OnChannelActive)}");
        }

        public void OnChannelException(ITcpSession session, Exception ex)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Server)}.{nameof(OnChannelException)}"+ex.ToString());
        }

        public void OnChannelInactive(ITcpSession session)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Server)}.{nameof(OnChannelInactive)}");
        }

        public void OnChannelRead(ITcpSession session, ref WrappedStreamReader reader, ref ChannelArgs args)
        {
             string str = System.Text.Encoding.UTF8.GetString(args.buffer.FirstSpan);
            Console.WriteLine("收到》》"+str);
            Console.WriteLine($"{nameof(TcpTestFilter_Server)}.{nameof(OnChannelRead)}");
        }

        public void OnChannelWrite(ITcpSession session, ref WrappedStreamWriter writer, ref ChannelArgs args)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Server)}.{nameof(OnChannelWrite)}");
        }
    }
}