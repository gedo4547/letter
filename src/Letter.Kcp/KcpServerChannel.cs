using System;
using System.Net;
using System.Threading.Tasks;

using Letter.IO;

namespace Letter.Kcp
{
    class KcpServerChannel : AKcpChannel<KcpServerOptions>, IKcpServerChannel
    {
        public KcpServerChannel(KcpServerOptions options, IKcpChannel channel, Action<IFilterPipeline<IKcpSession>> handler) 
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