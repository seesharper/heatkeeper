using HeatKeeper.Abstractions;
using HeatKeeper.Server.Schedules;
using LightInject;

namespace HeatKeeper.Server.Programs;

[Order(2)]
public class JanitorBootStrapper : IBootStrapper
{
    private readonly IServiceFactory _serviceFactory;

    public JanitorBootStrapper(IServiceFactory serviceFactory)
        => _serviceFactory = serviceFactory;

    public async Task Execute()
    {
        using (Scope scope = _serviceFactory.BeginScope())
        {
            ICommandExecutor commandExecutor = scope.GetInstance<ICommandExecutor>();
            await commandExecutor.ExecuteAsync(new AddAllSchedulesToJanitorCommand());
        }
    }
}