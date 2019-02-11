using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HeatKeeper.Server.Host
{
    public static class HttpResponseExtensions
    {
        public static async Task WriteProblemDetails<TProblemDetails>(this HttpResponse response, TProblemDetails problemDetails) where TProblemDetails: ProblemDetails
        {
            if (response.HasStarted)
            {
                return;
            }

            response.StatusCode = (int)problemDetails.Status;
            var json = JsonConvert.SerializeObject(problemDetails);
            await response.WriteAsync(json);

        }
    }
}