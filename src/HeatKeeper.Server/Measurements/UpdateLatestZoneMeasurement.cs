namespace HeatKeeper.Server.Measurements
{
    public class UpdateLatestMeasurementCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateLatestMeasurementCommand>
    {
        public async Task HandleAsync(UpdateLatestMeasurementCommand command, CancellationToken cancellationToken = default)
            => await dbConnection.ExecuteAsync(sqlProvider.UpdateLatestZoneMeasurement, command);
    }


    [RequireReporterRole]
    public class UpdateLatestMeasurementCommand : LatestZoneMeasurementCommand
    {
    }
}
