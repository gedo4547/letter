﻿using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Letter
{
    public sealed class ServerTlsOptions : TlsOptions
    {
        public ServerTlsOptions(X509Certificate certificate) : this(certificate, false)
        {
        }

        public ServerTlsOptions(X509Certificate certificate, bool negotiateClientCertificate) : this(certificate, negotiateClientCertificate, false)
        {
        }

        public ServerTlsOptions(X509Certificate certificate, bool negotiateClientCertificate, bool checkCertificateRevocation)
            : this(certificate, negotiateClientCertificate, checkCertificateRevocation, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12)
        {
        }

        public ServerTlsOptions(X509Certificate certificate, bool negotiateClientCertificate, bool checkCertificateRevocation, SslProtocols enabledProtocols)
            : base(enabledProtocols, checkCertificateRevocation)
        {
            this.Certificate = certificate;
            this.NegotiateClientCertificate = negotiateClientCertificate;
        }

        public X509Certificate Certificate { get; }

        public bool NegotiateClientCertificate { get; }
    }
}
