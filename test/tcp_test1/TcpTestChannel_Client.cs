using System;
using Letter;
using Letter.Tcp;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;


namespace tcp_test1
{
    public class TcpTestFilter_Client : ITcpChannelFilter
    {
        public async void OnTransportActive(ITcpSession session)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportActive)}" + session.Id);
            for (int i = 0; i < 10; i++)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes("你好"+i);
                session.Write(bytes);
            }

            await session.FlushAsync();
            // await session.DisposeAsync();
        }

        public void OnTransportException(ITcpSession session, Exception ex)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportException)}"+ex.ToString());
        }

        public void OnTransportInactive(ITcpSession session)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportInactive)}");
        }

        public void OnTransportRead(ITcpSession session, ref WrappedReader reader, object args)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportRead)}");
            List<ReadOnlySequence<byte>> buffers = (List<ReadOnlySequence<byte>>)args;
            for (int i = 0; i < buffers.Count; i++)
            {
                var buffer = buffers[i];
                string str = System.Text.Encoding.UTF8.GetString(buffer.FirstSpan);
                Console.WriteLine("收到》》"+str);
            }
        }

        public void OnTransportWrite(ITcpSession session, ref WrappedWriter writer, object args)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportWrite)}");
        }
    }
}