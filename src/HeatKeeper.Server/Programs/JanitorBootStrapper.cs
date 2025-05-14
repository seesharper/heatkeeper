using HeatKeeper.Abstractions;
using HeatKeeper.Server.Notifications;
using HeatKeeper.Server.Schedules;
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
            await commandExecutor.ExecuteAsync(new AddAllSchedulesToJanitorCommand());
            await commandExecutor.ExecuteAsync(new AddAllNotificationsToJanitorCommand());
        }
    }
}