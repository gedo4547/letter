using System;

namespace Letter.Kcp
{
    interface IEventSubscriber
    {
        void Register(RunnableUnitDelegate runnableUnit);

        void Unregister(RunnableUnitDelegate runnableUnit);
    }
}
