using System.Data;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Measurements;

[RequireReporterRole]
public record MaintainLatestSensorMeasurementCommand(MeasurementCommand[] Measurements);

public class MaintainLatestSensorMeasurementCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    : ICommandHandler<MaintainLatestSensorMeasurementCommand>
{
    public async Task HandleAsync(MaintainLatestSensorMeasurementCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var group in command.Measurements.GroupBy(m => new { m.SensorId, m.MeasurementType }))
        {
            var latest = group.OrderBy(m => m.Created).Last();
            await dbConnection.ExecuteAsync(sqlProvider.UpsertLatestSensorMeasurement, new
            {
                ExternalId = latest.SensorId,
                latest.MeasurementType,
                latest.Value,
                Updated = latest.Created
            });
        }
    }
}
