using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace common
{
    public sealed class SocketConfig
    {
        public readonly static bool useSsl = false;
        public readonly static X509Certificate2 cert = new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "dotnetty.com.pfx"), "password");
        
        public readonly static byte[] message = System.Text.Encoding.UTF8.GetBytes(Message.data_1024);
    }
}