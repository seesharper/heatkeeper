namespace HeatKeeper.Server.Locations;

public class UpdateDefaultInsideZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateDefaultInsideZoneCommand>
{
    public async Task HandleAsync(UpdateDefaultInsideZoneCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateDefaultInsideZone, command);
}

[RequireAdminRole]
public record UpdateDefaultInsideZoneCommand(long LocationId, long ZoneId);
