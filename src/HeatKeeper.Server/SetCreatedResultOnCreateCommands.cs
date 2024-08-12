using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server;

public class SetCreatedResultOnCreateCommands<TCommand>(ICommandHandler<TCommand> handler, IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<TCommand> where TCommand : ProblemCommand<Created<ResourceId>>
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);
        if (command.HasResult())
        {
            return;
        }
        var route = typeof(TCommand).GetCustomAttribute<PostAttribute>()?.Route;
        var resourceId = new ResourceId(await dbConnection.ExecuteScalarAsync<long>(sqlProvider.GetLastInsertedRowId));
        command.SetResult(TypedResults.Created(route, resourceId));
    }
}