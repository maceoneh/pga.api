using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.Exceptions
{
    internal class RegisterNotExistsException : Exception
    {
        public RegisterNotExistsException(string message) : base(message) { }
    }
}
