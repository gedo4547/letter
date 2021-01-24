using System;

namespace cocosocket4unity
{
    public interface IOutput
    {
        void output(ByteBuf msg, Kcp kcp, Object user);
    }
}
