using System;

namespace Pcf.GivingToCustomer.WebHost.Settings.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException()
        {
        }
        public BadRequestException(string message)
            : base(message)
        {
        }
        public BadRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
