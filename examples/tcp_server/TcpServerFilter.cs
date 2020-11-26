using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading.Tasks;
using common;
using Letter.IO;
using Letter.Tcp;

namespace tcp_server
{
    public sealed class TcpServerFilter : ITcpFilter
    {
        public void OnTransportActive(ITcpSession session)
        {
            System.Threading.Interlocked.Increment(ref ServerStatistics.client_count);
        }

        public void OnTransportInactive(ITcpSession session)
        {
            System.Threading.Interlocked.Decrement(ref ServerStatistics.client_count);
        }

        public void OnTransportException(ITcpSession session, Exception ex)
        {
            Console.WriteLine($"{session.Id} exception:{ex.ToString()}");
        }

        public void OnTransportRead(ITcpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            List<ReadOnlySequence<byte>> buffers = (List<ReadOnlySequence<byte>>)args.Value;
            for (int i = 0; i < buffers.Count; i++)
            {
                session.Write(SocketConfig.message);
            }

            session.FlushAsync().NoAwait();
        }

        public void OnTransportWrite(ITcpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
        }
    }
}