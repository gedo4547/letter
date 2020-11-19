namespace Letter.Tcp
{
    public class TcpServerOptions : ATcpOptions
    {
        public int Backlog { get; set; } = 512;
    }
}