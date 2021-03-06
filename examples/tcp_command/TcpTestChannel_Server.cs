﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using Letter.IO;
using Letter.Tcp;

namespace tcp_test1
{
    public class TcpTestFilter_Server : ITcpFilter
    {
        public void OnTransportActive(ITcpSession session)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Server)}.{nameof(OnTransportActive)}" + session.Id);
        }

        public void OnTransportException(ITcpSession session, Exception ex)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Server)}.{nameof(OnTransportException)}"+ex.ToString());
        }

        public void OnTransportInactive(ITcpSession session)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Server)}.{nameof(OnTransportInactive)}");
        }

        public void OnTransportRead(ITcpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            // Console.WriteLine(">>>>>>>>>>" + (args == null));
            List<ReadOnlySequence<byte>> buffers = (List<ReadOnlySequence<byte>>)args.Value;
            Console.WriteLine("收到包》》》》》"+buffers.Count);
            for (int i = 0; i < buffers.Count; i++)
            {
                var buffer = buffers[i];
                string str = System.Text.Encoding.UTF8.GetString(buffer.FirstSpan);
                Console.WriteLine("收到》》"+str);
            }
            
            Console.WriteLine($"{nameof(TcpTestFilter_Server)}.{nameof(OnTransportRead)}");
        }

        public void OnTransportWrite(ITcpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            // Console.WriteLine($"{nameof(TcpTestFilter_Server)}.{nameof(OnTransportWrite)}");
        }
    }
}