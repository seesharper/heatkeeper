using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Janitor;
using Microsoft.Extensions.Hosting;

namespace HeatKeeper.Server.Host.BackgroundTasks;

[ExcludeFromCodeCoverage]
public class JanitorHostedService : BackgroundService
{
    private readonly IJanitor _janitor;

    public JanitorHostedService(IJanitor janitor)
        => _janitor = janitor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!AppEnvironment.IsRunningFromTests)
        {
            await _janitor.Start(stoppingToken);
        }
    }
}