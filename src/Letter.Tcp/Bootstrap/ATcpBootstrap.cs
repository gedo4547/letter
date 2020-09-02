using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public abstract class ATcpBootstrap<TTcpOptions> : ABootstrap<ITcpChannel, ITcpSession, TTcpOptions>, ITcpBootstrap<TTcpOptions>
        where TTcpOptions : ATcpOptions
    {
        protected void Connected(ITcpSession session)
        {
            List<ITcpChannel> channels = this.GetChannelList();
            
            
            
            
        }

        
        public override Task StopAsync()
        {
            return base.StopAsync();
        }
    }
}