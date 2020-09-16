using System;

namespace Letter
{
    public interface IFilter<TContext, TReader, TWriter>
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
    {
        void OnFilterActive(TContext context);
        
        void OnFilterInactive(TContext context);
        
        void OnFilterException(TContext context, Exception ex);
        
        void OnFilterRead(TContext context, ref TReader reader, ref EventArgs args);
        
        void OnFilterWrite(TContext context, ref TWriter writer, ref EventArgs args);
    }
}