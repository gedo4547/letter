﻿using System;
using Letter;
using Letter.Tcp;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using Letter.IO;


namespace tcp_test1
{
    public class TcpTestFilter_Client : ITcpFilter
    {
        public void OnTransportActive(ITcpSession session)
        {
            M.session = session;
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportActive)}" + session.Id);
        }

        public void OnTransportException(ITcpSession session, Exception ex)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportException)}"+ex.ToString());
        }

        public void OnTransportInactive(ITcpSession session)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportInactive)}");
        }

        public void OnTransportRead(ITcpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportRead)}");
            List<ReadOnlySequence<byte>> buffers = (List<ReadOnlySequence<byte>>)args.Value;
            for (int i = 0; i < buffers.Count; i++)
            {
                var buffer = buffers[i];
                string str = System.Text.Encoding.UTF8.GetString(buffer.FirstSpan);
                Console.WriteLine("收到》》"+str);
            }
        }

        public void OnTransportWrite(ITcpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            // Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportWrite)}");
        }
    }
}