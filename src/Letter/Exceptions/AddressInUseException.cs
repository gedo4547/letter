using System;
using System.ComponentModel;

namespace Letter
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AddressInUseException : InvalidOperationException
    {
        public AddressInUseException(string message) : base(message)
        {
        }

        public AddressInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}