using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using common;
using Letter.IO;
using Letter.Tcp;

namespace tcp_client
{
    public class TcpClientFilter : ITcpFilter
    {
        public TcpClientFilter()
        {
            Program.startSend += OnStartSend;
        }

        public long count;
        private ITcpSession _session;
        
        private void OnStartSend()
        {
            _session.Write(common.SocketConfig.message);
            _session.FlushAsync().NoAwait();
        }

        public void OnTransportActive(ITcpSession session)
        {
            this._session = session;
        }

        public void OnTransportInactive(ITcpSession session)
        {
            
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
                count++;
                session.Write(SocketConfig.message);
            }

            session.FlushAsync().NoAwait();
        }

        public void OnTransportWrite(ITcpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
        }
    }
}