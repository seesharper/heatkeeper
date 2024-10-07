namespace HeatKeeper.Server.SetPoints.Api;

[RequireUserRole]
[Patch("api/setPoints/{SetPointId}")]
public record UpdateSetPointCommand(long SetPointId, double Value, double Hysteresis) : PatchCommand;

public class UpdateSetPointCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateSetPointCommand>
{
    public async Task HandleAsync(UpdateSetPointCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateSetPoint, command);
}