namespace HeatKeeper.Server.Lights.Api;

[RequireAdminRole]
[Delete("api/lights/{lightId}")]
public record DeleteLightCommand(long LightId) : DeleteCommand;

public class DeleteLight(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteLightCommand>
{
    public async Task HandleAsync(DeleteLightCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteLight, command);
}
