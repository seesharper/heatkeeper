using HeatKeeper.Server.Locations.Api;
using Microsoft.AspNetCore.Http;

namespace HeatKeeper.Server.Locations;

public class ValidateUpdateLocation(ICommandHandler<UpdateLocationCommand> handler, IQueryExecutor queryExecutor) : ICommandHandler<UpdateLocationCommand>
{
    public async Task HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken = default)
    {
        var locationExists = await queryExecutor.ExecuteAsync(new LocationExistsQuery(command.Name, command.Id), cancellationToken);
        if (locationExists)
        {
            command.SetResult(TypedResults.Problem($"Location {command.Name} already exists", statusCode: StatusCodes.Status409Conflict));
            return;
        }
        await handler.HandleAsync(command, cancellationToken);
    }
}