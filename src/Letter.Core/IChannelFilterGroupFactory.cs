using System;

namespace Letter
{
    public interface IChannelFilterGroupFactory<TSession, TFilter, TFilterGroup> : IAsyncDisposable
        where TSession : ISession
        where TFilter : IChannelFilter<TSession>
        where TFilterGroup : IChannelFilterGroup<TSession, TFilter>
    {
        void AddFilterFactory(Func<TFilter> FilterFactory);

        TFilterGroup CreateFilterGroup();
    }
}