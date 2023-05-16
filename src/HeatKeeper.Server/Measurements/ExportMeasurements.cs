using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Export;

namespace HeatKeeper.Server.Measurements;

[RequireBackgroundRole]
public record ExportMeasurementsCommand();

public class ExportMeasurementsCommandHandler : ICommandHandler<ExportMeasurementsCommand>
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IQueryExecutor _queryExecutor;

    public ExportMeasurementsCommandHandler(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
    {
        _commandExecutor = commandExecutor;
        _queryExecutor = queryExecutor;
    }

    public async Task HandleAsync(ExportMeasurementsCommand command, CancellationToken cancellationToken = default)
    {
        MeasurementToExport[] measurementsToExport = await _queryExecutor.ExecuteAsync(new MeasurementsToExportQuery(), cancellationToken);

        await _commandExecutor.ExecuteAsync(new ExportMeasurementsToInfluxDbCommand(measurementsToExport), cancellationToken);

        ExportedMeasurement[] exportedMeasurements = measurementsToExport.Select(m => new ExportedMeasurement(m.Id, DateTime.UtcNow)).ToArray();
        await _commandExecutor.ExecuteAsync(new UpdateExportedMeasurementsCommand(exportedMeasurements), cancellationToken);

        await _commandExecutor.ExecuteAsync(new DeleteExportedMeasurementsCommand(RetentionDate: DateTime.UtcNow.Subtract(TimeSpan.FromDays(1))), cancellationToken);
    }
}