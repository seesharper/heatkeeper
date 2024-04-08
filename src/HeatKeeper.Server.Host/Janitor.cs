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
                                 .WithName("SetChannelStates")
                                 .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken cancellationToken)
                                     => await commandExecutor.ExecuteAsync(new SetChannelStatesCommand(), cancellationToken))
                                 .WithSchedule(new CronSchedule(configuration.GetChannelStateCronExpression())))
                             .Schedule(builder => builder
                                 .WithName("DeleteExpiredMeasurements")
                                 .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken cancellationToken)
                                     => await commandExecutor.ExecuteAsync(new DeleteExpiredMeasurementsCommand(), cancellationToken))
                                 .WithSchedule(new CronSchedule(configuration.GetDeleteExpiredMeasurementsCronExpression())));
        });

        services.AddHostedService<JanitorHostedService>();
        return services;
    }
}