namespace HeatKeeper.Server.Heaters.Api;

[RequireAdminRole]
[Patch("api/heaters/{heaterId}")]
public record UpdateHeaterCommand(long HeaterId, string Name, string Description, string MqttTopic, string OnPayload, string OffPayload, bool Enabled) : PatchCommand;

public class UpdateHeater(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateHeaterCommand>
{
    public async Task HandleAsync(UpdateHeaterCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateHeater, command);
}

