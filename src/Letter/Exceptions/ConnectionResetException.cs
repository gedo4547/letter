using System;
using System.ComponentModel;
using System.IO;

namespace Letter
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ConnectionResetException : IOException
    {
        public ConnectionResetException(string message) : base(message)
        {
        }

        public ConnectionResetException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}