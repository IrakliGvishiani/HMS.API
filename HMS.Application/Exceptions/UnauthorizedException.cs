using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() { }

        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
