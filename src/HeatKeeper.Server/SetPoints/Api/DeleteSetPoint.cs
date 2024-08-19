namespace HeatKeeper.Server.SetPoints.Api;

[RequireUserRole]
[Delete("api/SetPoints/{SetPointId}")]
public record DeleteSetPointCommand(long SetPointId) : DeleteCommand;

public class DeleteSetPointCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteSetPointCommand>
{
    public async Task HandleAsync(DeleteSetPointCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteSetPoint, command);
}