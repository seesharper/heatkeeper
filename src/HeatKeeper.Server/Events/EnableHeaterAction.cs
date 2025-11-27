using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HeatKeeper.Server.Heaters;

namespace HeatKeeper.Server.Events;

public record EnableHeaterActionParameters(
    [property: Description("The ID of the heater to enable"), Required] long HeaterId);

[Action(4, "Enable Heater", "Enables a specific heater")]
public sealed class EnableHeaterAction(ICommandExecutor commandExecutor) : IAction<EnableHeaterActionParameters>
{
    public async Task ExecuteAsync(EnableHeaterActionParameters parameters, CancellationToken cancellationToken = default)
        => await commandExecutor.ExecuteAsync(new EnableHeaterCommand(parameters.HeaterId), cancellationToken);
}
