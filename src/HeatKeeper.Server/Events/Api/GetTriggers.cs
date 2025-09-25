using System.Text.Json;

namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Get("api/triggers")]
public record GetTriggersQuery : IQuery<TriggerDefinition[]>;

public class GetTriggers(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetTriggersQuery, TriggerDefinition[]>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task<TriggerDefinition[]> HandleAsync(GetTriggersQuery query, CancellationToken cancellationToken = default)
    {
        var triggerInfos = await dbConnection.ReadAsync<TriggerInfo>(sqlProvider.GetAllEventTriggers);
        var triggers = new List<TriggerDefinition>();

        foreach (var info in triggerInfos)
        {
            try
            {
                var definition = JsonSerializer.Deserialize<TriggerDefinition>(info.Definition, JsonOptions);
                if (definition != null)
                {
                    triggers.Add(definition);
                }
            }
            catch (JsonException)
            {
                // Skip invalid JSON definitions
                continue;
            }
        }

        return triggers.ToArray();
    }
}

public record TriggerInfo(long Id, string Name, string Definition);