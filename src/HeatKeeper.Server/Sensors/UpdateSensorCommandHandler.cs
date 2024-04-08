namespace HeatKeeper.Server.Sensors
{
    public class UpdateSensorCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateSensorCommand>
    {
        private readonly IDbConnection dbConnection = dbConnection;
        private readonly ISqlProvider sqlProvider = sqlProvider;

        public async Task HandleAsync(UpdateSensorCommand command, CancellationToken cancellationToken = default)
            => await dbConnection.ExecuteAsync(sqlProvider.UpdateSensor, command);
    }

    [RequireAdminRole]
    public record UpdateSensorCommand(long SensorId, string Name, string Description);
}
