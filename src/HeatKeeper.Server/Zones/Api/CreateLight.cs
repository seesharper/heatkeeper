namespace HeatKeeper.Server.Zones.Api;

[RequireAdminRole]
[Post("/api/zones/{ZoneId}/lights")]
public record CreateLightCommand(string Name, string Description, string MqttTopic, string OnPayload, string OffPayload, long ZoneId, bool Enabled = true) : PostCommand;

public class CreateLight(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateLightCommand>
{
    public async Task HandleAsync(CreateLightCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertLight, command);
}
