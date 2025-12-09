namespace HeatKeeper.Server.Lights.Api;

[RequireAdminRole]
[Patch("api/lights/{lightId}")]
public record UpdateLightCommand(long LightId, string Name, string Description, string MqttTopic, string OnPayload, string OffPayload, bool Enabled) : PatchCommand;

public class UpdateLight(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateLightCommand>
{
    public async Task HandleAsync(UpdateLightCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateLight, command);
}
