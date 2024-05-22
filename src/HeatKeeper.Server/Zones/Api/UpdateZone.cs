namespace HeatKeeper.Server.Zones.Api;

[RequireAdminRole]
[Patch("/api/zones/{ZoneId}")]
public record UpdateZoneCommand(long ZoneId, long LocationId, string Name, string Description);

public class UpdateZone(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateZoneCommand>
{
    public async Task HandleAsync(UpdateZoneCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateZone, command).ConfigureAwait(false);
}


