using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Heaters;

namespace HeatKeeper.Server.Events;

[Action(4, "Enable Heater", "Enables a specific heater")]
[RequireAdminRole]
public record EnableHeaterCommand(
    [property: Description("The ID of the heater to enable"), Required] long HeaterId);

public sealed class EnableHeaterCommandHandler(ICommandExecutor commandExecutor) : ICommandHandler<EnableHeaterCommand>
{
    public async Task HandleAsync(EnableHeaterCommand command, CancellationToken cancellationToken = default)
        => await commandExecutor.ExecuteAsync(new Heaters.EnableHeaterCommand(command.HeaterId), cancellationToken);
}
