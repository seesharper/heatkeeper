using HeatKeeper.Server.Exceptions;
using HeatKeeper.Server.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server.Locations;

public class ValidateInsertLocationCommand(ICommandHandler<InsertLocationCommand> handler, IQueryExecutor queryExecutor) : ICommandHandler<InsertLocationCommand>
{
    public async Task HandleAsync(InsertLocationCommand command, CancellationToken cancellationToken = default)
    {
        var locationExists = await queryExecutor.ExecuteAsync(new LocationExistsQuery(command.Name), cancellationToken);
        if (locationExists)
        {
            throw new HeatKeeperConflictException($"Location {command.Name} already exists");
            // command.SetResult(TypedResults.Problem($"Location {command.Name} already exists", statusCode: StatusCodes.Status409Conflict));
            // return;
        }
        await handler.HandleAsync(command, cancellationToken);
    }
}