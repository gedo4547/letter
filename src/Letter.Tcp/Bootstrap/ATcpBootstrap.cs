using Letter.IO;

namespace Letter.Tcp
{
    public abstract class ATcpBootstrap<TOptions> : ABootstrap<ITcpTransport, ITcpContext, ITcpSession, TOptions, ITcpChannel, WrappedStreamReader, WrappedStreamWriter>
        where TOptions : ATcpOptions
    {
        protected void OnConnect(ITcpSession session)
        {
            var channels = this.GetChannels();
            var transport = this.GetTransport();
            
            transport.Run(session, channels);
        }
    }
}