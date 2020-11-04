namespace System.Net.Sockets
{
    public readonly struct SocketResult
    {
        public SocketResult(int bytesTransferred, SocketError error)
        {
            this.error = error;
            this.bytesTransferred = bytesTransferred;
        }

        public readonly SocketError error;
        public readonly int bytesTransferred;
    }
}