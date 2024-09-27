using System.Net;

namespace PromoCodeFactory.WebHost.Models
{
    internal class ExceptionResponse
    {
        public HttpStatusCode StatusCode { get; init; }
        public string Message { get; init; }

        public ExceptionResponse(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}