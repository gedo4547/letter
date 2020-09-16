﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter
{
    public class AChannelFilterGroup<TSession, TFilter> : IChannelFilterGroup<TSession, TFilter>
        where TSession : ISession
        where TFilter : IChannelFilter<TSession>
    {
        public AChannelFilterGroup(List<TFilter> filters)
        {
            this.filters = filters;
        }

        protected List<TFilter> filters;
        
        public void OnFilterActive(TSession session)
        {
            int count = filters.Count;
            for (int i = 0; i < count; ++i)
            {
                this.filters[i].OnChannelActive(session);
            }
        }

        public void OnFilterInactive(TSession session)
        {
            int count = filters.Count;
            for (int i = 0; i < count; ++i)
            {
                this.filters[i].OnChannelInactive(session);
            }
        }

        public void OnFilterException(TSession session, Exception ex)
        {
            int count = this.filters.Count;
            for (int i = 0; i < count; ++i)
            {
                this.filters[i].OnChannelException(session, ex);
            }
        }
        
        public ValueTask DisposeAsync()
        {
            if (this.filters != null)
            {
                this.filters.Clear();
                this.filters = null;
            }

            return default;
        }
    }
}