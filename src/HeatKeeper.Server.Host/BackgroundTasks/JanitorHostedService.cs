using System.Diagnostics.CodeAnalysis;
using Janitor;

namespace HeatKeeper.Server.Host.BackgroundTasks;

[ExcludeFromCodeCoverage]
public class JanitorHostedService(IJanitor janitor) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!AppEnvironment.IsRunningFromTests)
        {
            await janitor.Start(stoppingToken);
        }
    }
}