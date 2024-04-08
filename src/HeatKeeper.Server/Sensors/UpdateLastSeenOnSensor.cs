namespace HeatKeeper.Server.Sensors;

[RequireReporterRole]
public record UpdateLastSeenOnSensorCommand(string ExternalId, DateTime LastSeen);

public class UpdateLastSeenOnSensorCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateLastSeenOnSensorCommand>
{
    public async Task HandleAsync(UpdateLastSeenOnSensorCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateLastSeenOnSensor, command);
}