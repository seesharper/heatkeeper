using System.Net;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using HeatKeeper.Server.Validation;
using System.Linq;

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
                    Title = "Authorization Failed",
                    Detail = context.Exception.Message
                };
                context.Result = new UnauthorizedObjectResult(problemDetails);
            }

            if (context.Exception is HeatKeeperConflictException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Title = "Conflict",
                    Detail = context.Exception.Message
                };
                context.Result = new ConflictObjectResult(problemDetails);
            }

            if (context.Exception is ValidationFailedException exception)
            {
                var validationProblemDetails = new ProblemDetails()
                {
                    Title = "Request Validation Error",
                    Detail = exception.ValidationErrors.First().ErrorMessage,
                    Status = (int?)HttpStatusCode.BadRequest,
                    Instance = context.HttpContext.TraceIdentifier
                };

                // var errorsGroupedByMemberName = exception.ValidationErrors.GroupBy(e => e.MemberName);
                // foreach (var errorGroup in errorsGroupedByMemberName)
                // {
                //     validationProblemDetails.Errors.Add(errorGroup.Key, errorGroup.Select(e => e.ErrorMessage).ToArray());
                // }

                context.Result = new BadRequestObjectResult(validationProblemDetails);
            }
        }
    }
}
