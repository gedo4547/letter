using System;

namespace Letter.Kcp
{
    public interface IKcpRunnable
    {
        void Update(ref DateTime nowTime);
    }
}