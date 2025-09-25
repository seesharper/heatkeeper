using System.Text.Json;

namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Post("api/triggers")]
public record PostTriggerCommand(TriggerDefinition Trigger) : PostCommand;

public class PostTrigger(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PostTriggerCommand>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task HandleAsync(PostTriggerCommand command, CancellationToken cancellationToken = default)
    {
        var definition = JsonSerializer.Serialize(command.Trigger, JsonOptions);

        var parameters = new
        {
            Name = command.Trigger.Name,
            Definition = definition
        };

        await dbConnection.ExecuteAsync(sqlProvider.InsertEventTrigger, parameters);
    }
}