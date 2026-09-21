using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Exceptions
{
    public class InternalServerException : Exception
    {
        public InternalServerException() { }
        public InternalServerException(string message) : base(message)
        {
        }
    
    }
}
