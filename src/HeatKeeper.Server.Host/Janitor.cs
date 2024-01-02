using CQRS.Command.Abstractions;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Server.Electricity;
using HeatKeeper.Server.Host.BackgroundTasks;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Programs;
using Janitor;

namespace HeatKeeper.Server.Host;

public static class JanitorServiceCollectionExtensions
{
    public static IServiceCollection AddJanitor(this IServiceCollection services)
    {
        services.AddJanitor((sp, config) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            config
                             .Schedule(builder => builder
                                 .WithName("ExportElectricalMarketPrices")
                                 .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken cancellationToken)
                                     => await commandExecutor.ExecuteAsync(new ExportAllMarketPricesCommand(), cancellationToken))
                                 .WithSchedule(new CronSchedule("0 15,18,21 * * *")))
                             .Schedule(builder => builder
                                 .WithName("ExportMeasurements")
                                 .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken cancellationToken)
                                     => await commandExecutor.ExecuteAsync(new ExportMeasurementsCommand(), cancellationToken))
                                 .WithSchedule(new CronSchedule("* * * * *")))
                             .Schedule(builder => builder
                                 .WithName("SetChannelStates")
                                 .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken cancellationToken)
                                     => await commandExecutor.ExecuteAsync(new SetChannelStatesCommand(), cancellationToken))
                                 .WithSchedule(new CronSchedule(configuration.GetChannelStateCronExpression())));
        });

        services.AddHostedService<JanitorHostedService>();
        return services;
    }
}