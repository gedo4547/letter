﻿using System;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    sealed class KcpServerBootstrap : AKcpBootstrap<KcpServerOptions>, IKcpServerBootstrap
    {
        protected override Task<IKcpChannel> ChannelFactoryAsync(KcpServerOptions options, Action<IFilterPipeline<IKcpSession>> handler)
        {
            throw new NotImplementedException();
        }
    }
}