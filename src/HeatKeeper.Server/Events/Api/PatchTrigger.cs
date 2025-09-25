using System.Text.Json;

namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Patch("api/triggers/{id}")]
public record PatchTriggerCommand(long Id, TriggerDefinition Trigger) : PatchCommand;

public class PatchTrigger(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PatchTriggerCommand>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task HandleAsync(PatchTriggerCommand command, CancellationToken cancellationToken = default)
    {
        var definition = JsonSerializer.Serialize(command.Trigger, JsonOptions);

        var parameters = new
        {
            Id = command.Id,
            Name = command.Trigger.Name,
            Definition = definition
        };

        await dbConnection.ExecuteAsync(sqlProvider.UpdateEventTrigger, parameters);
    }
}