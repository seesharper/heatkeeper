using System.Text.Json;

namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Post("api/triggers")]
public record PostTriggerCommand(string Name) : PostCommand;

public class PostTrigger(IDbConnection dbConnection) : ICommandHandler<PostTriggerCommand>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task HandleAsync(PostTriggerCommand command, CancellationToken cancellationToken = default)
    {
        // Create an empty TriggerDefinition with the provided name
        var emptyTriggerDefinition = new TriggerDefinition(
            command.Name,
            0, // Default event ID (0 = no event)
            null, // No condition — will be configured later
            new List<ActionBinding>() // Empty actions
        );

        var parameters = new
        {
            Name = command.Name,
            Definition = JsonSerializer.Serialize(emptyTriggerDefinition, JsonOptions)
        };

        await dbConnection.ExecuteAsync("INSERT INTO EventTriggers (Name, Definition) VALUES (@Name, @Definition)", parameters);
    }
}