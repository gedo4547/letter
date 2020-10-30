using System;
using Letter;
using Letter.Tcp;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading.Tasks;


namespace tcp_test1
{
    public class TcpTestFilter_Client : ITcpChannelFilter
    {
        public async void OnTransportActive(ITcpSession session)
        {
            M.session = session;
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportActive)}" + session.Id);
           
            
           
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

        public void OnTransportRead(ITcpSession session, ref WrappedReader reader, List<Object> args)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportRead)}");
            List<ReadOnlySequence<byte>> buffers = (List<ReadOnlySequence<byte>>)args[0];
            for (int i = 0; i < buffers.Count; i++)
            {
                var buffer = buffers[i];
                string str = System.Text.Encoding.UTF8.GetString(buffer.FirstSpan);
                Console.WriteLine("收到》》"+str);
            }
        }

        public void OnTransportWrite(ITcpSession session, ref WrappedWriter writer, List<Object> args)
        {
            // Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnTransportWrite)}");
        }
    }
}