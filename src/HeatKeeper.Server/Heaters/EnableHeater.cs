namespace HeatKeeper.Server.Heaters;

[RequireBackgroundRole]
public record EnableHeaterCommand(long HeaterId);

public class EnableHeater(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<EnableHeaterCommand>
{
    public async Task HandleAsync(EnableHeaterCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.EnableHeater, command);
}
