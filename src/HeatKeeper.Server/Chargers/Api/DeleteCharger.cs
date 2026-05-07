namespace HeatKeeper.Server.Chargers.Api;

[RequireAdminRole]
[Delete("api/chargers/{chargerId}")]
public record DeleteChargerCommand(long ChargerId) : DeleteCommand;

public class DeleteCharger(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteChargerCommand>
{
    public async Task HandleAsync(DeleteChargerCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteCharger, command);
}
