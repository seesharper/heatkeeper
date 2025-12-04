#nullable enable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Command for turning heaters off.
/// </summary>
[Action(2, "Turn Heaters Off", "Turns off all heaters in a specific zone for a specified reason")]
[RequireAdminRole]
public sealed class TurnHeatersOffCommand
{
    [Required]
    [Description("Which zone to target")]
    public required int ZoneId { get; init; }

    [Description("Optional reason")]
    public string? Reason { get; init; }
}

public sealed class TurnHeatersOffCommandHandler : ICommandHandler<TurnHeatersOffCommand>
{
    public Task HandleAsync(TurnHeatersOffCommand command, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[ACTION] TurnHeatersOff => ZoneId={command.ZoneId}, Reason='{command.Reason}'");
        // Place real device/API calls here
        return Task.CompletedTask;
    }
}