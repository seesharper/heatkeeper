using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Parameters for sending a notification.
/// </summary>
public sealed class SendNotificationParameters
{
    [Required]
    [Description("The notification message")]
    public required string Message { get; init; }

    [Description("The notification severity level")]
    public string? Severity { get; init; }
}

/// <summary>
/// Sample action that simulates sending notifications.
/// </summary>
[DisplayName("Send Notification")]
public sealed class SendNotificationAction : IAction<SendNotificationParameters>
{
    public Task ExecuteAsync(SendNotificationParameters parameters, CancellationToken ct)
    {
        Console.WriteLine($"[ACTION] Notify => {parameters.Severity} :: {parameters.Message}");
        return Task.CompletedTask;
    }
}