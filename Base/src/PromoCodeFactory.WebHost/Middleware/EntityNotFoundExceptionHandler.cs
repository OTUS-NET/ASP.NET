using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PromoCodeFactory.Core.Exceptions;
using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Middleware;

public class EntityNotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not EntityNotFoundException)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        httpContext.Response.ContentType = MediaTypeNames.Text.Plain;
        await httpContext.Response.WriteAsync(exception.Message, cancellationToken);

        return true;
    }
}
