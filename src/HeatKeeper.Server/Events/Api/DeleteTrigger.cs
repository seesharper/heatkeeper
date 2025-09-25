namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Delete("api/triggers/{id}")]
public record DeleteTriggerCommand(long Id) : DeleteCommand;

public class DeleteTrigger(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteTriggerCommand>
{
    public async Task HandleAsync(DeleteTriggerCommand command, CancellationToken cancellationToken = default)
    {
        await dbConnection.ExecuteAsync(sqlProvider.DeleteEventTrigger, new { Id = command.Id });
    }
}