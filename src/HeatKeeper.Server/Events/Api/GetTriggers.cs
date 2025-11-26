namespace HeatKeeper.Server.Events.Api;

public record TriggerInfo(long Id, string Name);

[RequireUserRole]
[Get("api/triggers")]
public record GetTriggersQuery : IQuery<TriggerInfo[]>;

public class GetTriggers(IDbConnection dbConnection) : IQueryHandler<GetTriggersQuery, TriggerInfo[]>
{

    public async Task<TriggerInfo[]> HandleAsync(GetTriggersQuery query, CancellationToken cancellationToken = default)
        => [.. await dbConnection.ReadAsync<TriggerInfo>("SELECT Id, Name FROM EventTriggers")];
}