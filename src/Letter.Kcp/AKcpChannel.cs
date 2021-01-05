using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    public abstract class AKcpChannel<TOptions> : AChannel<IKcpSession, TOptions>
        where TOptions : KcpOptions
    {
        public AKcpChannel(TOptions options, IKcpChannel channel, Action<IFilterPipeline<IKcpSession>> handler)
        {
            base.ConfigurationSelfOptions(options);
            base.ConfigurationSelfFilter(handler);
        }

        protected IKcpChannel channel;





    }
}