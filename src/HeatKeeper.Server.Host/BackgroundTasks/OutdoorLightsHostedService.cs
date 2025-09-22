using HeatKeeper.Server.Host;
using HeatKeeper.Server.Lighting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Host.BackgroundTasks;

/// <summary>
/// Background service that runs the outdoor lights controller on a timer.
/// Checks light state periodically and publishes events when needed.
/// </summary>
public class OutdoorLightsHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutdoorLightsHostedService> _logger;
    private readonly TimeSpan _checkInterval;

    public OutdoorLightsHostedService(IServiceProvider serviceProvider, ILogger<OutdoorLightsHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        // Get check interval from configuration via a scoped service
        using var scope = _serviceProvider.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
        _checkInterval = configuration.GetOutdoorLightsCheckInterval();

        _logger.LogInformation("Outdoor lights hosted service initialized with check interval {CheckInterval}", _checkInterval);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (AppEnvironment.IsRunningFromTests)
        {
            _logger.LogInformation("Skipping outdoor lights service - running from tests");
            return;
        }

        _logger.LogInformation("Starting outdoor lights monitoring service");

        // Initial check and publish
        await CheckLightState();

        // Set up periodic timer
        using var timer = new PeriodicTimer(_checkInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CheckLightState();
        }

        _logger.LogInformation("Outdoor lights monitoring service stopped");
    }

    private async Task CheckLightState()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var lightsController = scope.ServiceProvider.GetRequiredService<IOutdoorLightsController>();

            await lightsController.CheckAndPublishLightStates();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking outdoor light state");
        }
    }
}