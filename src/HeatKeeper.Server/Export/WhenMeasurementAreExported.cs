using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Sensors;

namespace HeatKeeper.Server.Export;

public class WhenMeasurementAreExported : ICommandHandler<ExportMeasurementsToInfluxDbCommand>
{
    private readonly ICommandHandler<ExportMeasurementsToInfluxDbCommand> _handler;
    private readonly ICommandExecutor _commandExecutor;

    public WhenMeasurementAreExported(ICommandHandler<ExportMeasurementsToInfluxDbCommand> handler, ICommandExecutor commandExecutor)
    {
        _handler = handler;
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(ExportMeasurementsToInfluxDbCommand command, CancellationToken cancellationToken = default)
    {
        await _handler.HandleAsync(command, cancellationToken);
        var measurementsGroupedByExternalSensorId = command.MeasurementsToExport.Select(mte => new { mte.ExternalSensorId, mte.Created }).GroupBy(mte => mte.ExternalSensorId);
        foreach (var group in measurementsGroupedByExternalSensorId)
        {
            var latestCreatedDate = group.OrderBy(cr => cr.Created).Last().Created;
            await _commandExecutor.ExecuteAsync(new UpdateLastSeenOnSensorCommand(group.Key, latestCreatedDate), cancellationToken);
        }
    }
}
