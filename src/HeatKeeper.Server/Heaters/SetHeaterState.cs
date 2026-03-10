namespace HeatKeeper.Server.Heaters;

[RequireBackgroundRole]
public record SetHeaterStateCommand(long HeaterId, HeaterState HeaterState);

public class SetHeaterState(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<SetHeaterStateCommand>
{
    public async Task HandleAsync(SetHeaterStateCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.SetHeaterState, command);
}
