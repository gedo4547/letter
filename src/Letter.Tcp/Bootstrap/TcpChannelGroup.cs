using System;
using System.Collections.Generic;

namespace Letter.Tcp
{
    public class TcpChannelGroup
    {
        public TcpChannelGroup(List<Func<ITcpChannel>> factorys)
        {
            for (int i = 0; i < factorys.Count; i++)
            {
                var channel = factorys[i]();
            }
        }
    }
}