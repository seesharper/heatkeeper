namespace HeatKeeper.Server.SetPoints;

[RequireAdminRole]
public record DeleteAllSetPointsCommand(long scheduleId);

public class DeleteAllSetPointsCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteAllSetPointsCommand>
{
    public async Task HandleAsync(DeleteAllSetPointsCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteAllSetPoints, command);
}