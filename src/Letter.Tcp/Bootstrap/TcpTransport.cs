using System.Collections.Generic;

namespace Letter.Tcp
{
    public class TcpTransport
    {
        private ITcpSession session;
        private List<ITcpChannel> channels;
        
        public TcpTransport(ITcpSession session, List<ITcpChannel> channels)
        {
            
        }
    }
}