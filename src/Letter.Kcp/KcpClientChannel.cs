using System;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    class KcpClientChannel : AKcpChannel<KcpClientOptions>, IKcpClientChannel
    {
        public KcpClientChannel(KcpClientOptions options, IKcpChannel channel, Action<IFilterPipeline<IKcpSession>> handler) 
            : base(options, channel, handler)
        {
        }

        public async Task StartAsync(EndPoint address)
        {
            await base.channel.BindAsync(new IPEndPoint(IPAddress.IPv6Any, 0));
        }

        public Task BindAsync(EndPoint address)
        {
            throw new NotImplementedException();
        }
    }
}