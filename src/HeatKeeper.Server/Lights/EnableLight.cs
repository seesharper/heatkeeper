namespace HeatKeeper.Server.Lights;

[RequireBackgroundRole]
public record EnableLightCommand(long LightId);

public class EnableLight(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<EnableLightCommand>
{
    public async Task HandleAsync(EnableLightCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.EnableLight, command);
}
