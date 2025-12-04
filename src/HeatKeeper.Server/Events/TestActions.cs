#nullable enable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Test command for sending notifications - used only in tests
/// </summary>
[Action(-1, "[TEST] Send Notification", "Sends a notification with a message and optional severity level (test action)")]
[RequireAdminRole]
public record TestSendNotificationCommand(
    [property: Description("The notification message"), Required] string Message,
    [property: Description("The notification severity level")] string? Severity = null);

public sealed class TestSendNotificationCommandHandler : ICommandHandler<TestSendNotificationCommand>
{
    public Task HandleAsync(TestSendNotificationCommand command, CancellationToken cancellationToken = default)
    {
        // Test implementation - just log
        System.Console.WriteLine($"[TEST ACTION] SendNotification => Message='{command.Message}', Severity='{command.Severity}'");
        return Task.CompletedTask;
    }
}

/// <summary>
/// Test command for turning heaters off - used only in tests
/// </summary>
[Action(-2, "[TEST] Turn Heaters Off", "Turns off heaters in a specified zone with an optional reason (test action)")]
[RequireAdminRole]
public sealed class TestTurnHeatersOffCommand
{
    [Required]
    [Description("Which zone to target")]
    public required int ZoneId { get; init; }

    [Description("Optional reason")]
    public string? Reason { get; init; }
}

public sealed class TestTurnHeatersOffCommandHandler : ICommandHandler<TestTurnHeatersOffCommand>
{
    public Task HandleAsync(TestTurnHeatersOffCommand command, CancellationToken cancellationToken = default)
    {
        // Test implementation - just log
        System.Console.WriteLine($"[TEST ACTION] TurnHeatersOff => ZoneId={command.ZoneId}, Reason='{command.Reason}'");
        return Task.CompletedTask;
    }
}

/// <summary>
/// Test command for disabling heater - used only in tests
/// </summary>
[Action(-100, "[TEST] Disable Heater", "Disables a specific heater for testing (test action)")]
[RequireAdminRole]
public record TestDisableHeaterCommand(
    [property: Description("The ID of the heater to disable"), Required] long HeaterId,
    [property: Description("The reason for disabling")] int DisabledReason = 0);

public sealed class TestDisableHeaterCommandHandler : ICommandHandler<TestDisableHeaterCommand>
{
    public Task HandleAsync(TestDisableHeaterCommand command, CancellationToken cancellationToken = default)
    {
        // Test implementation - just log
        System.Console.WriteLine($"[TEST ACTION] DisableHeater => HeaterId={command.HeaterId}, Reason={command.DisabledReason}");
        return Task.CompletedTask;
    }
}
