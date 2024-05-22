namespace HeatKeeper.Server.Heaters.Api;

[RequireAdminRole]
[Delete("api/heaters/{heaterId}")]
public record DeleteHeaterCommand(long HeaterId) : DeleteCommand;

public class DeleteHeater(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteHeaterCommand>
{
    public async Task HandleAsync(DeleteHeaterCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteHeater, command);
}