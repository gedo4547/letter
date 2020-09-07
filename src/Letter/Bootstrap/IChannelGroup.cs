using System;
using System.Collections.Generic;

namespace Letter
{
    public interface IChannelGroup<TChannel>
    {
        void Initialize(List<Func<TChannel>> channelFactorys);
    }
}