using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Heaters;

namespace HeatKeeper.Server.Events;

[Action(6, "Set Heater State", "Sets the state of a specific heater")]
[RequireBackgroundRole]
public sealed class SetHeaterStateActionCommand
{
   
    [Required]
    [Description("Location")]
    [Lookup("api/locations")]
    public long LocationId { get; init; }

    [Required]
    [Description("Heater")]
    [Lookup("api/locations/{locationId}/heaters")]
    public long HeaterId { get; init; }

    [Required]
    [Description("Heater state")]
    [Lookup("api/heaters/states")]
    public HeaterState HeaterState { get; init; }
}

public sealed class SetHeaterStateActionCommandHandler(ICommandExecutor commandExecutor) : ICommandHandler<SetHeaterStateActionCommand>
{
    public async Task HandleAsync(SetHeaterStateActionCommand command, CancellationToken cancellationToken = default)
        => await commandExecutor.ExecuteAsync(new Heaters.SetHeaterStateCommand(command.HeaterId, command.HeaterState), cancellationToken);
}
