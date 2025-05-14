using System.Diagnostics.CodeAnalysis;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Messaging;
using HeatKeeper.Server.PushSubscriptions;

namespace HeatKeeper.Server.Host.BackgroundTasks;

[ExcludeFromCodeCoverage]
public class MessageBusHostedService(IMessageBus messageBus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        messageBus.Subscribe<SendPushNotificationCommand>(async (SendPushNotificationCommand command, ICommandExecutor commandExecutor) =>
            {
                await commandExecutor.ExecuteAsync(command);
            });

        if (!AppEnvironment.IsRunningFromTests)
        {
            await messageBus.Start();
        }
    }
}