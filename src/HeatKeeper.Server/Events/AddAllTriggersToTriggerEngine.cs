namespace HeatKeeper.Server.Events;

[RequireBackgroundRole]
public record AddAllTriggersToTriggerEngineCommand();

public class AddAllTriggersToTriggerEngine(IQueryExecutor queryExecutor, TriggerEngine triggerEngine) : ICommandHandler<AddAllTriggersToTriggerEngineCommand>
{
    public async Task HandleAsync(AddAllTriggersToTriggerEngineCommand command, CancellationToken cancellationToken = default)
    {
        var allTriggers = await queryExecutor.ExecuteAsync(new GetAllEventTriggersQuery(), cancellationToken);
        triggerEngine.SetTriggers(allTriggers.Select(t => t.Definition));
    }
}
