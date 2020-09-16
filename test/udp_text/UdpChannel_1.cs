using System;
using System.Net;
using Letter;
using Letter.Udp;

namespace udp_text
{
    public class UdpFilter_1 : IUdpChannelFilter
    {
        public void OnChannelActive(IUdpSession session)
        {
            throw new NotImplementedException();
        }

        public void OnChannelInactive(IUdpSession session)
        {
            throw new NotImplementedException();
        }

        public void OnChannelException(IUdpSession session, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void OnChannelRead(IUdpSession session, EndPoint remoteAddress, ref WrappedDgramReader reader, ref ChannelArgs args)
        {
            throw new NotImplementedException();
        }

        public void OnChannelWrite(IUdpSession session, EndPoint remoteAddress, ref WrappedDgramWriter writer, ref ChannelArgs args)
        {
            throw new NotImplementedException();
        }
    }
}