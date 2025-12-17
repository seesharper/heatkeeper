namespace HeatKeeper.Server.Events;


[RequireBackgroundRole]
public record GetAllEventTriggersQuery() : IQuery<TriggerEvent[]>;

public class GetAllEventTriggers(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetAllEventTriggersQuery, TriggerEvent[]>
{
    public async Task<TriggerEvent[]> HandleAsync(GetAllEventTriggersQuery query, CancellationToken cancellationToken = default)
    {
        return (await dbConnection.ReadAsync<TriggerEvent>(sqlProvider.GetAllEventTriggers, query)).ToArray();
    }
}

public record TriggerEvent(
    long Id,
    string Name,
    TriggerDefinition Definition
);