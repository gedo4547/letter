using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;

namespace System.Net.Sockets
{
    public abstract class SocketSender : SocketBase
    {
        public SocketSender(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
        }
        
        private List<ArraySegment<byte>> _bufferList;
        
        protected List<ArraySegment<byte>> GetBufferList(in ReadOnlySequence<byte> buffer)
        {
            Debug.Assert(!buffer.IsEmpty);
            Debug.Assert(!buffer.IsSingleSegment);

            if (_bufferList == null)
            {
                _bufferList = new List<ArraySegment<byte>>();
            }
            else
            {
                // Buffers are pooled, so it's OK to root them until the next multi-buffer write.
                _bufferList.Clear();
            }

            foreach (var b in buffer)
            {
                _bufferList.Add(b.GetArray());
            }

            return _bufferList;
        }

       
    }
}