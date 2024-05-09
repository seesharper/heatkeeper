namespace HeatKeeper.Server.Locations;

public class UpdateDefaultOutsideZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateDefaultOutsideZoneCommand>
{
    public async Task HandleAsync(UpdateDefaultOutsideZoneCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateDefaultOutsideZone, command);
}

[RequireAdminRole]
public record UpdateDefaultOutsideZoneCommand(long LocationId, long ZoneId);
