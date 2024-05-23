namespace HeatKeeper.Server.Zones.Api;

[RequireAdminRole]
[Delete("/api/zones/{zoneId}")]
public record DeleteZoneCommand(long ZoneId) : DeleteCommand;

public class DeleteZone(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteZoneCommand>
{
    public async Task HandleAsync(DeleteZoneCommand command, CancellationToken cancellationToken = default)
    {
        await dbConnection.ExecuteAsync(sqlProvider.ClearZoneFromAllSensors, command);
        await dbConnection.ExecuteAsync(sqlProvider.ClearZoneFromAllLocations, command);
        await dbConnection.ExecuteAsync(sqlProvider.DeleteZone, command);
    }
}