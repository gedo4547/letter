using Letter.IO;

namespace Letter.Tcp
{
    public class TcpClientBootstrapOptions : TcpClientOptions
    {
        public BinaryOrder Order { get; } = BinaryOrder.BigEndian;
    }
}