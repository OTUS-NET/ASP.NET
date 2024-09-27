using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails pd = exception switch
        {
            TableIsEmptyException => ShapeProblemDetails(TableIsEmptyException.ErrorMessage, (int)EmployeeIsNotFoundException.StatusCode),
            EmployeeIsNotFoundException => ShapeProblemDetails(EmployeeIsNotFoundException.ErrorMessage, (int)EmployeeIsNotFoundException.StatusCode),
            RoleIsNotFoundException => ShapeProblemDetails(RoleIsNotFoundException.ErrorMessage, (int)RoleIsNotFoundException.StatusCode),
            _ => ShapeProblemDetails("An unexpected error has occured.", StatusCodes.Status500InternalServerError)
        };

        await httpContext.Response.WriteAsJsonAsync(pd, cancellationToken);

        ProblemDetails ShapeProblemDetails(string title, int statusCode) =>
        new()
        {
            Title = title,
            Type = exception.GetType().Name,
            Status = statusCode,
            Detail = exception.Message
        };

        return true;
    }
}