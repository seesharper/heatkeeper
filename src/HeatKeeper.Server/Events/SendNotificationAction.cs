namespace HeatKeeper.Server.Events;

/// <summary>
/// Sample action that simulates sending notifications.
/// </summary>
public sealed class SendNotificationAction : IAction
{
    public static ActionInfo GetActionInfo() => new()
    {
        Name = "SendNotification",
        DisplayName = "Send Notification",
        ParameterSchema = new[]
        {
            new ActionParameter("Message", "string", true),
            new ActionParameter("Severity", "string", false)
        }
    };

    public Task ExecuteAsync(IReadOnlyDictionary<string, object> parameters, CancellationToken ct)
    {
        var severity = parameters.TryGetValue("Severity", out var s) ? s : null;
        var message = parameters.TryGetValue("Message", out var m) ? m : null;
        Console.WriteLine($"[ACTION] Notify => {severity} :: {message} ");
        return Task.CompletedTask;
    }
}