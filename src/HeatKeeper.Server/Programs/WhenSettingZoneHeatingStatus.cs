using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Export;

namespace HeatKeeper.Server.Programs;

public class WhenSettingZoneHeatingStatus(ICommandHandler<SetZoneHeatingStatusCommand> handler, ICommandExecutor commandExecutor) : ICommandHandler<SetZoneHeatingStatusCommand>
{
    public async Task HandleAsync(SetZoneHeatingStatusCommand command, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);
        // TODO Create a measurement for the heating status
        // Find the heaters for this zone and update their status
        // Or use the heater name as the sensor name
        // In the end we want to know how often the heating is on and off
    }
}