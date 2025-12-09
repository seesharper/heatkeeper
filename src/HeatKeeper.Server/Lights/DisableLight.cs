namespace HeatKeeper.Server.Lights;

[RequireBackgroundRole]
public record DisableLightCommand(long LightId);

public class DisableLight(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DisableLightCommand>
{
    public async Task HandleAsync(DisableLightCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DisableLight, command);
}
