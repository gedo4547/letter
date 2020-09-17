using System.Collections.Generic;

namespace Letter
{
    public class StreamChannelFilterGroupFactory<TSession, TFilter> : AChannelFilterGroupFactory<TSession, TFilter, StreamChannelFilterGroup<TSession, TFilter>>, IChannelFilterGroupFactory<TSession,TFilter, StreamChannelFilterGroup<TSession, TFilter>>
        where TSession : ISession
        where TFilter : IStreamChannelFilter<TSession>
    {
        protected override StreamChannelFilterGroup<TSession, TFilter> FilterGroupCreator(List<TFilter> filters)
        {
            return new StreamChannelFilterGroup<TSession, TFilter>(filters);
        }
    }
}