using System;

namespace Letter
{
    public interface IChannel<TContext, TReader, TWriter>
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
    {
        void OnTransportActive(TContext context);
        
        void OnTransportInactive(TContext context);
        
        void OnTransportException(TContext context, Exception ex);
        
        void OnTransportRead(TContext context, ref TReader reader, ref TransportArgs args);
        
        void OnTransportWrite(TContext context, ref TWriter writer, ref TransportArgs args);
    }
}