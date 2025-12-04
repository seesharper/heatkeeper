#nullable enable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Command for sending a notification.
/// </summary>
[Action(1, "Send Notification", "Sends a notification with a message and optional severity level")]
[RequireAdminRole]
public sealed class SendNotificationCommand
{
    [Required]
    [Description("The notification message")]
    public required string Message { get; init; }

    [Description("The notification severity level")]
    public string? Severity { get; init; }
}

public sealed class SendNotificationCommandHandler : ICommandHandler<SendNotificationCommand>
{
    public Task HandleAsync(SendNotificationCommand command, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[ACTION] Notify => {command.Severity} :: {command.Message}");
        return Task.CompletedTask;
    }
}