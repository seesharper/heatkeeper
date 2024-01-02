using System.Net;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Exceptions;
using HeatKeeper.Server.Validation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is AuthenticationFailedException)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Title = exception.Message
            };
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        if (exception is AuthorizationFailedException)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Title = "Authorization Failed",
                Detail = exception.Message
            };
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        if (exception is HeatKeeperConflictException)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = "Conflict",
                Detail = exception.Message
            };
            httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        if (exception is ValidationFailedException validationFailedException)
        {
            var validationProblemDetails = new ProblemDetails()
            {
                Title = "Request Validation Error",
                Detail = validationFailedException.ValidationErrors.First().ErrorMessage,
                Status = (int?)HttpStatusCode.BadRequest,
                Instance = httpContext.TraceIdentifier
            };

            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
            return true;
        }

        return false;
    }
}