using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Sensors;

namespace HeatKeeper.Server.Export;

public class WhenMeasurementAreExported : ICommandHandler<ExportMeasurementsCommand>
{
    private readonly ICommandHandler<ExportMeasurementsCommand> _handler;
    private readonly ICommandExecutor _commandExecutor;

    public WhenMeasurementAreExported(ICommandHandler<ExportMeasurementsCommand> handler, ICommandExecutor commandExecutor)
    {
        _handler = handler;
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(ExportMeasurementsCommand command, CancellationToken cancellationToken = default)
    {
        await _handler.HandleAsync(command, cancellationToken);
        var measurementsGroupedByExternalSensorId = command.MeasurementsToExport.Select(mte => new { mte.ExternalSensorId, mte.Created }).GroupBy(mte => mte.ExternalSensorId);
        foreach (var group in measurementsGroupedByExternalSensorId)
        {
            DateTime latestCreatedDate = group.OrderBy(cr => cr.Created).Last().Created;
            await _commandExecutor.ExecuteAsync(new UpdateLastSeenOnSensorCommand(group.Key, latestCreatedDate), cancellationToken);
        }
    }
}