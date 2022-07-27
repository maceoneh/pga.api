using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.Exceptions
{
    public class RegisterExistsException : Exception
    {
        public RegisterExistsException(string message): base(message)
        { 
            
        }
    }
}
