
namespace Letter
{
    public interface IDgramChannel<TContext> : IChannel<TContext>
        where TContext : IContext
    {
        void OnChannelRead(TContext context, ref WrappedDgramReader reader, ref ChannelArgs args);
        
        void OnChannelWrite(TContext context, ref WrappedDgramWriter writer, ref ChannelArgs args);
    }
}