using System;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    class KcpClientChannel : AKcpChannel<KcpClientOptions>, IKcpClientChannel
    {
        public KcpClientChannel(KcpClientOptions options, IUdpChannel channel, Action<IFilterPipeline<IKcpSession>> handler) 
            : base(options, channel, handler)
        {
        }

        public Task StartAsync(EndPoint address)
        {
            throw new NotImplementedException();
        }

        public Task BindAsync(EndPoint address)
        {
            throw new NotImplementedException();
        }
    }
}