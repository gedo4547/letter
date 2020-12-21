using System;
using System.IO.Pipelines;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp.Common
{
    public sealed class DefaultSingleController : IChannelController
    {
        private IKcpSession session;
        
        public bool RegisterSession(IKcpSession session)
        {
            if (this.session != null)
            {
                throw new Exception();
            }

            this.session = session;
            return true;
        }
        
        public bool UnregisterSession(IKcpSession session)
        {
            if (this.session == null)
            {
                return false;
            }

            if (this.session != session)
            {
                return false;
            }

            this.session = null;
            return true;
        }
        
        public void OnRcvUdpMessage(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            if (this.session != null)
            {
                var buffer = reader.ReadBuffer((int)reader.Length);
                
            }
        }

        public void OnSndUdpMessage(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}