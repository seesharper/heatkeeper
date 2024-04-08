namespace HeatKeeper.Server.Sensors
{
    public class CreateSensorCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateSensorCommand>
    {
        public async Task HandleAsync(CreateSensorCommand command, CancellationToken cancellationToken = default)
            => await dbConnection.ExecuteAsync(sqlProvider.InsertSensor, command);
    }

    [RequireReporterRole]
    public record CreateSensorCommand(string ExternalId, string Name, string Description, DateTime lastSeen);
}
