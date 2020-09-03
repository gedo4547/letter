using System;

namespace Letter.Box
{
    public interface IChannel<TContext, TReader, TWriter>
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
    {
        void OnTransportActive(TContext context);
        
        void OnTransportInactive(TContext context);
        
        void OnTransportException(TContext context, Exception ex);
        
        void OnTransportRead(TContext context, ref TReader reader, ref MessageArgs args);
        
        void OnTransportWrite(TContext context, ref TWriter writer, ref MessageArgs args);
    }
}