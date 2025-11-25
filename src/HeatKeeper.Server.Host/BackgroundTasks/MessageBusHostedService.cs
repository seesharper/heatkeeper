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
                if (eventMessage.State == LightState.On)
                {
                    await eventBus.PublishAsync(new SunriseEvent(eventMessage.LocationName), CancellationToken.None);
                }
                else if (eventMessage.State == LightState.Off)
                {
                    await eventBus.PublishAsync(new SunsetEvent(eventMessage.LocationName), CancellationToken.None);
                }
            });

        if (!AppEnvironment.IsRunningFromTests)
        {
            await messageBus.Start();
        }
    }
}