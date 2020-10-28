using System.Security.Authentication;

namespace Letter.Tcp
{
    public abstract class SslOptions
    {
        protected SslOptions(SslProtocols enabledProtocols, bool checkCertificateRevocation)
        {
            this.EnabledProtocols = enabledProtocols;
            this.CheckCertificateRevocation = checkCertificateRevocation;
        }

        public SslProtocols EnabledProtocols { get; }
        public bool CheckCertificateRevocation { get; }
    }
}
