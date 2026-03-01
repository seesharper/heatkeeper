using System.Text.Json;

namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Get("api/triggers/{TriggerId}")]
public record TriggerDetailsQuery(long TriggerId) : IQuery<TriggerDefinition>;

public class GetTriggerDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<TriggerDetailsQuery, TriggerDefinition>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task<TriggerDefinition> HandleAsync(TriggerDetailsQuery query, CancellationToken cancellationToken = default)
    {
        var result = await dbConnection.ReadAsync<TriggerRow>(sqlProvider.GetEventTrigger, new { id = query.TriggerId });
        var triggerRow = result.SingleOrDefault();

        if (triggerRow == null)
            throw new InvalidOperationException($"Trigger with ID {query.TriggerId} was not found.");

        if (string.IsNullOrEmpty(triggerRow.Definition))
            return new TriggerDefinition(triggerRow.Name, 0, null, []);

        return JsonSerializer.Deserialize<TriggerDefinition>(triggerRow.Definition, JsonOptions)!;
    }
}

internal record TriggerRow(long Id, string Name, string Definition);