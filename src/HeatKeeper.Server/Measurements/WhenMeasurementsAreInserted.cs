using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Sensors;

namespace HeatKeeper.Server.Measurements;

public class WhenMeasurementsAreInserted : ICommandHandler<MeasurementCommand[]>
{
    private readonly ICommandHandler<MeasurementCommand[]> _handler;
    private readonly ICommandExecutor _commandExecutor;

    public WhenMeasurementsAreInserted(ICommandHandler<MeasurementCommand[]> handler, ICommandExecutor commandExecutor)
    {
        _handler = handler;
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(MeasurementCommand[] measurements, CancellationToken cancellationToken = default)
    {
        await _commandExecutor.ExecuteAsync(new CreateMissingSensorsCommand(measurements.Select(mc => mc.SensorId)), cancellationToken);
        await _handler.HandleAsync(measurements, cancellationToken);
        await _commandExecutor.ExecuteAsync(new MaintainLatestZoneMeasurementCommand(measurements), cancellationToken);
    }
}
