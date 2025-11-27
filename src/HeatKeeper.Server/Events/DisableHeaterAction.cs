using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HeatKeeper.Server.Heaters;

namespace HeatKeeper.Server.Events;

public record DisableHeaterActionParameters(
    [property: Description("The ID of the heater to disable"), Required] long HeaterId);

[Action(5, "Disable Heater", "Disables a specific heater")]
public sealed class DisableHeaterAction : IAction<DisableHeaterActionParameters>
{
    private readonly ICommandExecutor _commandExecutor;

    public DisableHeaterAction(ICommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task ExecuteAsync(DisableHeaterActionParameters parameters, CancellationToken cancellationToken = default)
    {
        await _commandExecutor.ExecuteAsync(new DisableHeaterCommand(parameters.HeaterId), cancellationToken);
    }
}
