using System;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    class KcpServerChannel : AKcpChannel<KcpServerOptions>, IKcpServerChannel
    {
        public KcpServerChannel(KcpServerOptions options, IUdpChannel channel, Action<IFilterPipeline<IKcpSession>> handler) 
            : base(options, channel, handler)
        {
        }

        public Task StartAsync(EndPoint address)
        {
            throw new NotImplementedException();
        }
    }
}