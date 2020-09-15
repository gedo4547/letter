
namespace Letter
{
    public interface IStreamChannel<TContext> : IChannel<TContext> where TContext : IContext
    {
        void OnChannelRead(TContext context, ref WrappedStreamReader reader, ref ChannelArgs args);
        
        void OnChannelWrite(TContext context, ref WrappedStreamWriter writer, ref ChannelArgs args);
    }
}