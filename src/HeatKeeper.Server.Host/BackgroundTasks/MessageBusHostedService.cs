using System.Diagnostics.CodeAnalysis;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Events;
using HeatKeeper.Server.Lighting;
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

        messageBus.Subscribe<OutdoorLightStateChanged>(async (OutdoorLightStateChanged eventMessage, IEventBus eventBus) =>
            {
                await eventBus.PublishAsync(DomainEvent<OutdoorLightStateChanged>.Create(eventMessage), CancellationToken.None);
            });

        if (!AppEnvironment.IsRunningFromTests)
        {
            await messageBus.Start();
        }
    }
}