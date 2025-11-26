using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Parameters for turning heaters off.
/// </summary>
public sealed class TurnHeatersOffParameters
{
    [Required]
    [Description("Which zone to target")]
    public required int ZoneId { get; init; }

    [Description("Optional reason")]
    public string? Reason { get; init; }
}

/// <summary>
/// Sample action that simulates turning heaters off in a specified zone.
/// </summary>
[Action(2, "Turn Heaters Off", "Turns off heaters in a specified zone with an optional reason")]
public sealed class TurnHeatersOffAction : IAction<TurnHeatersOffParameters>
{
    public Task ExecuteAsync(TurnHeatersOffParameters parameters, CancellationToken ct)
    {
        Console.WriteLine($"[ACTION] TurnHeatersOff => ZoneId={parameters.ZoneId}, Reason='{parameters.Reason}'");
        // Place real device/API calls here
        return Task.CompletedTask;
    }
}