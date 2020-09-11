using System;

namespace Letter
{
    public interface IChannel<TContext, TReader, TWriter>
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
    {
        void OnChannelActive(TContext context);
        
        void OnChannelInactive(TContext context);
        
        void OnChannelException(TContext context, Exception ex);
        
        void OnChannelRead(TContext context, ref TReader reader, ref EventArgs args);
        
        void OnChannelWrite(TContext context, ref TWriter writer, ref EventArgs args);
    }
}