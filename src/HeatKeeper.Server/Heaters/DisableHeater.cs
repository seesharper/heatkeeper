namespace HeatKeeper.Server.Heaters;

[RequireBackgroundRole]
public record DisableHeaterCommand(long HeaterId);

public class DisableHeater(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DisableHeaterCommand>
{
    public async Task HandleAsync(DisableHeaterCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DisableHeater, command);
}
