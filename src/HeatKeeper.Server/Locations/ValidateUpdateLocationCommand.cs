using HeatKeeper.Server.Exceptions;

namespace HeatKeeper.Server.Locations;

public class ValidateUpdateLocationCommand(ICommandHandler<UpdateLocationCommand> handler, IQueryExecutor queryExecutor) : ICommandHandler<UpdateLocationCommand>
{
    public async Task HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken = default)
    {
        var locationExists = await queryExecutor.ExecuteAsync(new LocationExistsQuery(command.Name, command.Id), cancellationToken);
        if (locationExists)
        {
            throw new HeatKeeperConflictException($"Location {command.Name} already exists");
        }
        await handler.HandleAsync(command, cancellationToken);
    }
}