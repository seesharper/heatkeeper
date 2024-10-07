namespace HeatKeeper.Server.Locations.Api;

[RequireAdminRole]
[Patch("api/locations/{locationId}")]
public record UpdateLocationCommand(long Id, string Name, string Description, long? DefaultOutsideZoneId, long? DefaultInsideZoneId) : PatchCommand;

public class UpdateLocation(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateLocationCommand>
{
    public async Task HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateLocation, command);
}


