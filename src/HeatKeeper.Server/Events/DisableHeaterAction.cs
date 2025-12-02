using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HeatKeeper.Server.Heaters;

namespace HeatKeeper.Server.Events;

public record DisableHeaterActionParameters(
    [property: Description("The ID of the heater to disable"), Required] long HeaterId);

[Action(5, "Disable Heater", "Disables a specific heater")]
public sealed class DisableHeaterAction(ICommandExecutor commandExecutor) : IAction<DisableHeaterActionParameters>
{
    public async Task ExecuteAsync(DisableHeaterActionParameters parameters, CancellationToken cancellationToken = default)
        => await commandExecutor.ExecuteAsync(new DisableHeaterCommand(parameters.HeaterId), cancellationToken);
}
