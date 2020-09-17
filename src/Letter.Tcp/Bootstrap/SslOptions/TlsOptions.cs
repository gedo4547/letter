﻿using System.Security.Authentication;

namespace Letter
{
    public abstract class TlsOptions
    {
        protected TlsOptions(SslProtocols enabledProtocols, bool checkCertificateRevocation)
        {
            this.EnabledProtocols = enabledProtocols;
            this.CheckCertificateRevocation = checkCertificateRevocation;
        }

        public SslProtocols EnabledProtocols { get; }
        public bool CheckCertificateRevocation { get; }
    }
}
