using System.Collections.Generic;

namespace Letter
{
    public class DgramChannelFilterGroupFactory<TSession, TFilter> : AChannelFilterGroupFactory<TSession, TFilter, DgramChannelFilterGroup<TSession, TFilter>>, IChannelFilterGroupFactory<TSession, TFilter, DgramChannelFilterGroup<TSession, TFilter>>
        where TSession : ISession
        where TFilter : IDgramChannelFilter<TSession>
    {
        protected override DgramChannelFilterGroup<TSession, TFilter> FilterGroupCreator(List<TFilter> filters)
        {
            return new DgramChannelFilterGroup<TSession, TFilter>(filters);
        }
    }
}