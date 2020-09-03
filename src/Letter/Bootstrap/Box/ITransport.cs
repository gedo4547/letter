using System.Collections.Generic;

namespace Letter.Box
{
    public interface ITransport<TSession, TChannel, TContext, TReader, TWriter>
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TSession : ISession
        where TChannel : IChannel<TContext, TReader, TWriter>
    {
        public void Run(TSession session, List<TChannel> channels);
    }
}