using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    public abstract class ATcpBootstrap<TOptions> : ABootstrap<TOptions, TcpFilterGroup, ITcpFilter, ITcpContext, WrappedStreamReader, WrappedStreamWriter>, ITcpBootstrap<TOptions>
        where TOptions: ATcpOptions
    {
        protected BinaryOrder order;

        public abstract Task StartAsync(EndPoint point);

        protected override TcpFilterGroup OnCreateFilterGroup(List<ITcpFilter> Filters)
        {
            return new TcpFilterGroup(Filters);
        }

        protected void OnConnect(ITcpClient client)
        {
            TcpContext context = new TcpContext(this.FilterGroupFactory.CreateFilterGroup(), this.order);
            
            context.Initialize(client);
        }
    }
}