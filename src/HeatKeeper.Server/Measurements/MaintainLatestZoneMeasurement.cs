using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Measurements;

[RequireReporterRole]
public record MaintainLatestZoneMeasurementCommand(MeasurementCommand[] Measurements);

public class MaintainLatestZoneMeasurementCommandHandler : ICommandHandler<MaintainLatestZoneMeasurementCommand>
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IQueryExecutor _queryExecutor;

    public MaintainLatestZoneMeasurementCommandHandler(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
    {
        _commandExecutor = commandExecutor;
        _queryExecutor = queryExecutor;
    }

    public async Task HandleAsync(MaintainLatestZoneMeasurementCommand command, CancellationToken cancellationToken = default)
    {
        var measurementsGroupedByExternalSensorId = command.Measurements.GroupBy(m => new { m.SensorId, m.MeasurementType });

        foreach (var group in measurementsGroupedByExternalSensorId)
        {
            var latestMeasurement = group.OrderBy(m => m.Created).Last();
            var zoneId = await _queryExecutor.ExecuteAsync(new ZoneByExternalSensorQuery(latestMeasurement.SensorId), cancellationToken);

            if (zoneId == null)
            {
                return;
            }

            var latestZoneMeasurementExists = await _queryExecutor.ExecuteAsync(new LatestZoneMeasurementExistsQuery() { ZoneId = zoneId.Value, MeasurementType = latestMeasurement.MeasurementType }, cancellationToken);
            if (latestZoneMeasurementExists)
            {
                await _commandExecutor.ExecuteAsync(new UpdateLatestMeasurementCommand() { MeasurementType = latestMeasurement.MeasurementType, ZoneId = zoneId.Value, Value = latestMeasurement.Value, Updated = latestMeasurement.Created }, cancellationToken);
            }
            else
            {
                await _commandExecutor.ExecuteAsync(new InsertLatestZoneMeasurementCommand() { MeasurementType = latestMeasurement.MeasurementType, ZoneId = zoneId.Value, Value = latestMeasurement.Value, Updated = latestMeasurement.Created }, cancellationToken);
            }
        }
    }
}