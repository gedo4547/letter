using Letter.IO;

namespace Letter
{
    public interface IStreamChannel<TContext> : IChannel<TContext>
        where TContext : class, IContext
    {
        void OnChannelRead(TContext context, ref WrappedStreamReader reader, ref EventArgs args);
        
        void OnChannelWrite(TContext context, ref WrappedStreamWriter writer, ref EventArgs args);
    }
}