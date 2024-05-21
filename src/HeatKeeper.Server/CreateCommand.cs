using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server;

public record CreateCommand : Command<Results<Created<ResourceId>, ProblemHttpResult>>
{
    public void SetCreatedResult(ResourceId resourceId)
    {
        var route = GetType().GetCustomAttribute<PostAttribute>()?.Route;
        SetResult(TypedResults.Created(route, resourceId));
    }
}
