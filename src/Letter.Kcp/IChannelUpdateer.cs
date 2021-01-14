using System;

namespace Letter.Kcp
{
    interface IChannelUpdateer
    {
        void Register(RunnableUnitDelegate runnableUnit);

        void Unregister(RunnableUnitDelegate runnableUnit);
    }
}
