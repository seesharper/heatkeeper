using HeatKeeper.Server.Exceptions;
using Microsoft.AspNetCore.Http;

namespace HeatKeeper.Server.Locations;

public class ValidateCreateLocation(ICommandHandler<CreateLocationCommand> handler, IQueryExecutor queryExecutor) : ICommandHandler<CreateLocationCommand>
{
    public async Task HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken = default)
    {
        var locationExists = await queryExecutor.ExecuteAsync(new LocationExistsQuery(command.Name), cancellationToken);
        if (locationExists)
        {
            command.SetResult(TypedResults.Problem($"Location {command.Name} already exists", statusCode: StatusCodes.Status409Conflict));
            return;
        }
        await handler.HandleAsync(command, cancellationToken);
    }
}