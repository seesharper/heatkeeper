namespace HeatKeeper.Server.Chargers;

[RequireBackgroundRole]
public record SetChargerStateCommand(long ChargerId, ChargerState ChargerState);

public class SetChargerState(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<SetChargerStateCommand>
{
    public async Task HandleAsync(SetChargerStateCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.SetChargerState, command);
}
