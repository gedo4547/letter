using System;
using System.Buffers;
using System.Buffers.Binary;
using System.ComponentModel;
using System.IO.Pipelines;
using System.Net;
using System.Runtime.CompilerServices;

using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    public abstract class AKcpController : IKcpClosable, IDisposable
    {
        public AKcpController()
        {
            var order = KcpHelpr.GetKcpBinaryOrder();
            this.binaryOrderOperators = KcpHelpr.GetOperators();
        }

        private IKcpSessionBuilder creator;
        private IBinaryOrderOperators binaryOrderOperators;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void SetCreator(IKcpSessionBuilder creator)
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
                {
                    return false;
                }

                return this.creator.IsActivate;
            }
        }

        protected IBinaryOrderOperators OrderOperators
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get  { return this.binaryOrderOperators; }
        }

        public abstract void OnSessionClosed(IKcpSession session);

        protected IKcpSession Create(uint conv, EndPoint remoteAddress)
        {
            if (this.creator == null)
            {
                throw new Exception("Controller is not initialized correctly");
            }

            return this.creator.Build(conv, remoteAddress, this);
        }

        protected void SendKcpMessageTo(IKcpSession session, ref ReadOnlySequence<byte> buffer)
        {
            var kcpSession = session as KcpSession;
            if (kcpSession == null)
            {
                throw new Exception("Unknown session type");
            }

            kcpSession.InputKcpMessage(ref buffer);
        }

        protected void SendUdpMessageTo(IKcpSession session, ref ReadOnlySequence<byte> buffer)
        {
            var kcpSession = session as KcpSession;
            if (kcpSession == null)
            {
                throw new Exception("Unknown session type");
            }

            kcpSession.InputUdpMessage(ref buffer);
        }

        protected void SendExceptionTo(IKcpSession session, Exception ex)
        {
            var kcpSession = session as KcpSession;
            if (kcpSession == null)
            {
                throw new Exception("Unknown session type");
            }

            kcpSession.OnUdpMessageException(ex);
        }

        public abstract void OnUdpActive(IUdpSession session);
        public abstract void OnUdpInactive(IUdpSession session);
        public abstract void OnUdpException(IUdpSession session, Exception ex);
        public abstract void OnUdpMessageInput(IUdpSession session, ref WrappedReader reader, WrappedArgs args);
        public abstract void OnUdpMessageOutput(IUdpSession session, ref WrappedWriter writer, WrappedArgs args);

        public virtual void Dispose()
        {
            this.creator = null;
            this.binaryOrderOperators = null;
        }
    }
}
