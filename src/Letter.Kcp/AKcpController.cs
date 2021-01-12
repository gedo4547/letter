using System;
using System.Buffers;
using System.Buffers.Binary;
using System.ComponentModel;
using System.IO.Pipelines;
using System.Net;

using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    public abstract class AKcpController : IKcpClosable, IDisposable
    {
        public AKcpController()
        {
            var order = KcpHelpr.KcpGlobalBinaryOrder;
            this.binaryOrderOperators = BinaryOrderOperatorsFactory.GetOperators(order);
        }

        private IKcpSessionCreator creator;
        private IBinaryOrderOperators binaryOrderOperators;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void SetCreator(IKcpSessionCreator creator)
        {
            if(this.creator != null)
                throw new Exception("Invalid assignment");

            this.creator = creator;
        }

        protected bool IsActivate 
        {
            get 
            {
                if (this.creator == null)
                    return false;

                return this.creator.IsActivate;
            }
        }

        protected IBinaryOrderOperators OrderOperators
        {
            get  { return this.binaryOrderOperators; }
        }

        public abstract void OnSessionClosed(IKcpSession session);

        protected IKcpSession Create(uint conv, EndPoint remoteAddress)
        {
            if(this.creator == null)
                throw new Exception("Controller is not initialized correctly");
            return this.creator.Create(conv, remoteAddress, this);
        }


        protected void SendKcpMessageTo(IKcpSession session, ref ReadOnlySequence<byte> buffer)
        {
            var kcpSession = session as KcpSession;
            kcpSession.InputKcpMessage(ref buffer);
        }

        protected void SendUdpMessageTo(IKcpSession session, ref ReadOnlySequence<byte> buffer)
        {
            var kcpSession = session as KcpSession;
            kcpSession.InputUdpMessage(ref buffer);
        }

        protected void SendExceptionTo(IKcpSession session, Exception ex)
        {
            var kcpSession = session as KcpSession;
            kcpSession.OnUdpMessageException(ex);
        }

        public abstract void OnUdpException(IUdpSession session, Exception ex);
        public abstract void OnUdpInput(IUdpSession session, ref WrappedReader reader, WrappedArgs args);
        public abstract void OnUdpOutput(IUdpSession session, ref WrappedWriter writer, WrappedArgs args);

        public virtual void Dispose()
        {
            this.creator = null;
            this.binaryOrderOperators = null;
        }
    }
}
