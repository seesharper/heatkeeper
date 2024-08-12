namespace HeatKeeper.Server.Zones.Api;

[RequireAdminRole]
[Post("/api/zones/{ZoneId}/heaters")]
public record CreateHeaterCommand(string Name, string Description, string MqttTopic, string OnPayload, string OffPayload, long ZoneId) : PostCommand;

public class CreateHeater(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateHeaterCommand>
{
    public async Task HandleAsync(CreateHeaterCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertHeater, command);
}



