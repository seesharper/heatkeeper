using System.Net;
using HeatKeeper.Server.Exceptions;
using HeatKeeper.Server.Security;
using HeatKeeper.Server.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace HeatKeeper.Server.Host
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is AuthenticationFailedException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Title = context.Exception.Message
                };
                context.Result = new UnauthorizedObjectResult(problemDetails);
            }

            if (context.Exception is AuthorizationFailedException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Title = context.Exception.Message
                };
                context.Result = new UnauthorizedObjectResult(problemDetails);
            }

            if (context.Exception is HeatKeeperConflictException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Title = context.Exception.Message
                };
                context.Result = new ConflictObjectResult(problemDetails);
            }
        }
    }
}
