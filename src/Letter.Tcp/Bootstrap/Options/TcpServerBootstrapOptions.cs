using Letter.IO;

namespace Letter.Tcp
{
    public class TcpServerBootstrapOptions : TcpServerOptions
    {
        public BinaryOrder Order { get; } = BinaryOrder.BigEndian;
    }
}