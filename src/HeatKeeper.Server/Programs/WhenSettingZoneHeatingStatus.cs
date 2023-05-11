using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Export;

namespace HeatKeeper.Server.Programs;

public class WhenSettingZoneHeatingStatus : ICommandHandler<SetZoneHeatingStatusCommand>
{
    private readonly ICommandHandler<SetZoneHeatingStatusCommand> _handler;
    private readonly ICommandExecutor _commandExecutor;

    public WhenSettingZoneHeatingStatus(ICommandHandler<SetZoneHeatingStatusCommand> handler, ICommandExecutor commandExecutor)
    {
        _handler = handler;
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(SetZoneHeatingStatusCommand command, CancellationToken cancellationToken = default)
    {
        await _handler.HandleAsync(command, cancellationToken);
        await _commandExecutor.ExecuteAsync(new ExportHeatingStatusToInfluxDbCommand(command.ZoneId, command.HeatingStatus, DateTime.UtcNow), cancellationToken);
    }
}