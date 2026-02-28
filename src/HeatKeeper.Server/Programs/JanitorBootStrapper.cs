using HeatKeeper.Abstractions;
using HeatKeeper.Server.Lighting;
using HeatKeeper.Server.Notifications;
using HeatKeeper.Server.Schedules;
using HeatKeeper.Server.SmartMeter;
using LightInject;

namespace HeatKeeper.Server.Programs;

[Order(2)]
public class JanitorBootStrapper(IServiceFactory serviceFactory) : IBootStrapper
{
    public async Task Execute()
    {
        using (Scope scope = serviceFactory.BeginScope())
        {
            var commandExecutor = scope.GetInstance<ICommandExecutor>();
            var timeProvider = scope.GetInstance<TimeProvider>();
            await commandExecutor.ExecuteAsync(new AddAllSchedulesToJanitorCommand());
            await commandExecutor.ExecuteAsync(new AddAllNotificationsToJanitorCommand());
            await commandExecutor.ExecuteAsync(new ScheduleSunriseAndSunsetEventsCommand(DateOnly.FromDateTime(timeProvider.GetUtcNow().Date)));
            await commandExecutor.ExecuteAsync(new ReScheduleSunriseAndSunsetEventsCommand());
            await commandExecutor.ExecuteAsync(new ScheduleSmartMeterReadingsPublishingCommand());
        }
    }
}