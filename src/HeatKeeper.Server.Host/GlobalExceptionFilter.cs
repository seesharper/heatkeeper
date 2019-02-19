using System.Net;
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
                ProblemDetails problemDetails = new ProblemDetails();
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Title = context.Exception.Message;
                context.Result = new UnauthorizedObjectResult(problemDetails);
            }
        }
    }
}