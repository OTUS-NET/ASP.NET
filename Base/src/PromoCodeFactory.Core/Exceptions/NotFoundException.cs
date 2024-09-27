using System;
using System.Net;

namespace PromoCodeFactory.Core.Exceptions
{
    public abstract class NotFoundException : Exception
    {
        protected NotFoundException(string message)
        {
            ErrorMessage = message;
        }

        public static HttpStatusCode StatusCode => HttpStatusCode.NotFound;
        public static string ErrorMessage;
    }
}
