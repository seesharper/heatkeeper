using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Heaters;

namespace HeatKeeper.Server.Events;

[Action(5, "Disable Heater", "Disables a specific heater")]
[RequireAdminRole]
public record DisableHeaterCommand(
    [property: Description("The ID of the heater to disable"), Required] long HeaterId,
    [property: Description("The reason for disabling the heater"), Required] HeaterDisabledReason DisabledReason);

public sealed class DisableHeaterCommandHandler(ICommandExecutor commandExecutor) : ICommandHandler<DisableHeaterCommand>
{
    public async Task HandleAsync(DisableHeaterCommand command, CancellationToken cancellationToken = default)
        => await commandExecutor.ExecuteAsync(new Heaters.DisableHeaterCommand(command.HeaterId, command.DisabledReason), cancellationToken);
}
