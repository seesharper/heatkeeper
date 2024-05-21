using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Zones;
using Microsoft.AspNetCore.Http;

namespace HeatKeeper.Server.Locations;

public class ValidateCreateZone(ICommandHandler<CreateZoneCommand> handler, IQueryExecutor queryExecutor) : ICommandHandler<CreateZoneCommand>
{
    public async Task HandleAsync(CreateZoneCommand command, CancellationToken cancellationToken = default)
    {
        var zoneExists = await queryExecutor.ExecuteAsync(new ZoneExistsQuery(command.LocationId, command.Name), cancellationToken);
        if (zoneExists)
        {
            command.SetResult(TypedResults.Problem($"Zone {command.Name} already exists", statusCode: StatusCodes.Status409Conflict));
            return;
        }
        await handler.HandleAsync(command, cancellationToken);
    }
}