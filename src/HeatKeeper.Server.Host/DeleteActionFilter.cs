using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HeatKeeper.Server.Host
{
    public class DeleteActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method == "DELETE")
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }
        }
    }
}
