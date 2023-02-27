using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Measurements;

namespace HeatKeeper.Server.Programs;

public class AfterTemperatureMeasurementHasBeenInserted : ICommandHandler<MeasurementCommand>
{
    private readonly ICommandHandler<MeasurementCommand> _handler;

    public AfterTemperatureMeasurementHasBeenInserted(ICommandHandler<MeasurementCommand> handler)
    {
        _handler = handler;
    }

    public async Task HandleAsync(MeasurementCommand command, CancellationToken cancellationToken = default)
    {
        await _handler.HandleAsync(command, cancellationToken);
        if (command.MeasurementType == MeasurementType.Temperature)
        {
            
            
            // 1. Find the target temperature for the zone associated with the sensor
            // 2.  If the target temperature is below target temperature - hysteresis , request the channel to be open
            //     If the target temperature is above target temperature + hysteresis, request the channel to close.
            //     If the target 
        }
    }
}