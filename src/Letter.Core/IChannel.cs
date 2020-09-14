using System;

namespace Letter
{
    public interface IChannel<TContext>
        where TContext : class, IContext
    {
        void OnChannelActive(TContext context);
        
        void OnChannelInactive(TContext context);
        
        void OnChannelException(TContext context, Exception ex);
    }
}