using System;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    class KcpClientChannel : AChannel<IKcpSession, KcpClientOptions>, IKcpClientChannel
    {
        public KcpClientChannel(KcpClientOptions options, IUdpChannel udpChannel, Action<IFilterPipeline<IKcpSession>> handler)
        {
            
        }
        
        public Task StartAsync(EndPoint address)
        {
            throw new NotImplementedException();
        }
    }
}