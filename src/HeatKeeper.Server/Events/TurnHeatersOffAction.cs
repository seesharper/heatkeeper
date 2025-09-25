namespace HeatKeeper.Server.Events;

/// <summary>
/// Sample action that simulates turning heaters off in a specified zone.
/// </summary>
public sealed class TurnHeatersOffAction : IAction
{
    public static ActionInfo GetActionInfo() => new()
    {
        Name = "TurnHeatersOff",
        DisplayName = "Turn Heaters Off",
        ParameterSchema = new[]
        {
            new ActionParameter("ZoneId", "number", true, "Which zone to target"),
            new ActionParameter("Reason", "string", false, "Optional reason"),
        }
    };

    public Task ExecuteAsync(IReadOnlyDictionary<string, object> parameters, CancellationToken ct)
    {
        var zoneId = parameters.TryGetValue("ZoneId", out var z) ? z : null;
        var reason = parameters.TryGetValue("Reason", out var r) ? r : null;
        Console.WriteLine($"[ACTION] TurnHeatersOff => ZoneId={zoneId}, Reason='{reason}'");
        // Place real device/API calls here
        return Task.CompletedTask;
    }
}