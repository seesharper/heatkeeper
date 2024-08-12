namespace HeatKeeper.Server.Locations.Api;

public class CreateZone(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateZoneCommand>
{
    public async Task HandleAsync(CreateZoneCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertZone, command);
}

[RequireUserRole]
[Post("api/locations/{locationId}/zones")]
public record CreateZoneCommand(long LocationId, string Name, string Description) : PostCommand;

