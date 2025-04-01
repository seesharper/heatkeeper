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
        if (!AppEnvironment.IsRunningFromTests)
        {
            messageBus.Subscribe<SendPushNotificationCommand>(async (SendPushNotificationCommand command, ICommandExecutor commandExecutor) =>
            {
                await commandExecutor.ExecuteAsync(command);
            });

            await messageBus.Start();
        }
    }
}