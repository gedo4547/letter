using System;

namespace Letter
{
    public interface IChannel<TContext> where TContext : IContext
    {
        void OnChannelActive(TContext context);
        
        void OnChannelInactive(TContext context);
        
        void OnChannelException(TContext context, Exception ex);
    }
}