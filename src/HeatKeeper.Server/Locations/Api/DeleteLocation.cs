using HeatKeeper.Server.Zones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server.Locations.Api;

[RequireAdminRole]
[Delete("api/locations/{locationId}")]
public record DeleteLocationCommand(long LocationId) : Command<NoContent>;

public class DeleteLocation(IDbConnection dbConnection, ISqlProvider sqlProvider, ICommandExecutor commandExecutor, IQueryExecutor queryExecutor) : ICommandHandler<DeleteLocationCommand>
{
    public async Task HandleAsync(DeleteLocationCommand command, CancellationToken cancellationToken = default)
    {
        // Remove all location users.
        // await commandExecutor.ExecuteAsync(new RemoveAllUsersFromLocationCommand(command.LocationId), cancellationToken);
        await dbConnection.ExecuteAsync(sqlProvider.DeleteAllUsersFromLocation, command);

        // await commandExecutor.ExecuteAsync(new RemoveAllZonesFromLocationCommand(command.LocationId), cancellationToken);

        // Remove all zones for this location.
        ZoneInfo[] zones = await queryExecutor.ExecuteAsync(new ZonesByLocationQuery(command.LocationId), cancellationToken);
        foreach (var zone in zones)
        {
            await commandExecutor.ExecuteAsync(new DeleteZoneCommand(zone.Id), cancellationToken);
        }

        //NOTE: We need to delete all programs 

        await dbConnection.ExecuteAsync(sqlProvider.DeleteLocation, command);

        command.SetResult(TypedResults.NoContent());
    }
}
