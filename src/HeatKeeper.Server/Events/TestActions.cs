#nullable enable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Test action for sending notifications - used only in tests
/// </summary>
public record TestSendNotificationParameters(
    [property: Description("The notification message"), Required] string Message,
    [property: Description("The notification severity level")] string? Severity = null);

[Action(-1, "[TEST] Send Notification", "Sends a notification with a message and optional severity level (test action)")]
public sealed class TestSendNotificationAction : IAction<TestSendNotificationParameters>
{
    public Task ExecuteAsync(TestSendNotificationParameters parameters, CancellationToken cancellationToken = default)
    {
        // Test implementation - just log
        System.Console.WriteLine($"[TEST ACTION] SendNotification => Message='{parameters.Message}', Severity='{parameters.Severity}'");
        return Task.CompletedTask;
    }
}

/// <summary>
/// Test action for turning heaters off - used only in tests
/// </summary>
public sealed class TestTurnHeatersOffParameters
{
    [Required]
    [Description("Which zone to target")]
    public required int ZoneId { get; init; }

    [Description("Optional reason")]
    public string? Reason { get; init; }
}

[Action(-2, "[TEST] Turn Heaters Off", "Turns off heaters in a specified zone with an optional reason (test action)")]
public sealed class TestTurnHeatersOffAction : IAction<TestTurnHeatersOffParameters>
{
    public Task ExecuteAsync(TestTurnHeatersOffParameters parameters, CancellationToken ct)
    {
        // Test implementation - just log
        System.Console.WriteLine($"[TEST ACTION] TurnHeatersOff => ZoneId={parameters.ZoneId}, Reason='{parameters.Reason}'");
        return Task.CompletedTask;
    }
}

/// <summary>
/// Test action for disabling heater - used only in tests
/// </summary>
public record TestDisableHeaterParameters(
    [property: Description("The ID of the heater to disable"), Required] long HeaterId,
    [property: Description("The reason for disabling")] int DisabledReason = 0);

[Action(-100, "[TEST] Disable Heater", "Disables a specific heater for testing (test action)")]
public sealed class TestDisableHeaterAction : IAction<TestDisableHeaterParameters>
{
    public Task ExecuteAsync(TestDisableHeaterParameters parameters, CancellationToken cancellationToken = default)
    {
        // Test implementation - just log
        System.Console.WriteLine($"[TEST ACTION] DisableHeater => HeaterId={parameters.HeaterId}, Reason={parameters.DisabledReason}");
        return Task.CompletedTask;
    }
}
