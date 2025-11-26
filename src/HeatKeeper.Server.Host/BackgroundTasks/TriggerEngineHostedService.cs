using System.Diagnostics.CodeAnalysis;
using HeatKeeper.Server.Events;

namespace HeatKeeper.Server.Host.BackgroundTasks;

[ExcludeFromCodeCoverage]
public class TriggerEngineHostedService(TriggerEngine triggerEngine) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!AppEnvironment.IsRunningFromTests)
        {
            await triggerEngine.StartAsync(stoppingToken);
        }
    }
}