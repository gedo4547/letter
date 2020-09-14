using Letter.IO;

namespace Letter
{
    public interface IDgramChannel<TContext> : IChannel<TContext>
        where TContext : class, IContext
    {
        void OnChannelRead(TContext context, ref WrappedDgramReader reader, ref EventArgs args);
        
        void OnChannelWrite(TContext context, ref WrappedDgramWriter writer, ref EventArgs args);
    }
}