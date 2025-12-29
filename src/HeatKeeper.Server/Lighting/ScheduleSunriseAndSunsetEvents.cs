
using HeatKeeper.Server.Events;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Yr;
using Janitor;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Lighting;

[RequireBackgroundRole]
public record ScheduleSunriseAndSunsetEventsCommand(DateTime DateTimeUtc);

public class ScheduleSunriseAndSunsetEvents(IQueryExecutor queryExecutor, TimeProvider timeProvider, IJanitor janitor, ILogger<ScheduleSunriseAndSunsetEvents> logger) : ICommandHandler<ScheduleSunriseAndSunsetEventsCommand>
{
    public async Task HandleAsync(ScheduleSunriseAndSunsetEventsCommand command, CancellationToken cancellationToken = default)
    {
        var locationCoordinates = await queryExecutor.ExecuteAsync(new GetLocationCoordinatesQuery(), cancellationToken);
        foreach (var location in locationCoordinates)
        {
            var sunEvents = await queryExecutor.ExecuteAsync(new GetSunEventsQuery((double)location.Latitude, (double)location.Longitude, DateOnly.FromDateTime(command.DateTimeUtc)), cancellationToken);
            if (sunEvents.SunriseUtc > timeProvider.GetUtcNow())
            {
                logger.LogInformation("Scheduling sunrise event for location {LocationId} at {SunriseTime}", location.Id, sunEvents.SunriseUtc);
                janitor.Schedule(builder =>
                {
                    logger.LogInformation("Scheduling sunrise event for location {LocationId} at {SunriseTime}", location.Id, sunEvents.SunriseUtc);
                    builder
                        .WithName($"Sunrise_Location_{location.Id}")
                        .WithSchedule(new RunOnceSchedule(sunEvents.SunriseUtc))
                        .WithScheduledTask((IEventBus eventBus) => eventBus.PublishAsync(new SunriseEvent(location.Id)));
                });
                janitor.Schedule(builder =>
                {
                    logger.LogInformation("Scheduling sunrise event (second try) for location {LocationId} at {SunriseTime}", location.Id, sunEvents.SunriseUtc.AddMinutes(10));
                    builder
                    .WithName($"Sunrise_Location_{location.Id}_second_try")
                    .WithSchedule(new RunOnceSchedule(sunEvents.SunriseUtc.AddMinutes(10)))
                    .WithScheduledTask((IEventBus eventBus) => eventBus.PublishAsync(new SunriseEvent(location.Id)));
                });
            }

            if (sunEvents.SunsetUtc > timeProvider.GetUtcNow())
            {
                logger.LogInformation("Scheduling sunset event for location {LocationId} at {SunsetTime}", location.Id, sunEvents.SunsetUtc);
                janitor.Schedule(builder =>
                {
                    logger.LogInformation("Scheduling sunset event for location {LocationId} at {SunsetTime}", location.Id, sunEvents.SunsetUtc);
                    builder
                        .WithName($"Sunset_Location_{location.Id}")
                        .WithSchedule(new RunOnceSchedule(sunEvents.SunsetUtc))
                        .WithScheduledTask((IEventBus eventBus) => eventBus.PublishAsync(new SunsetEvent(location.Id)));
                });
                janitor.Schedule(builder =>
                {
                    logger.LogInformation("Scheduling sunset event (second try) for location {LocationId} at {SunsetTime}", location.Id, sunEvents.SunsetUtc.AddMinutes(10));
                    builder
                    .WithName($"Sunset_Location_{location.Id}_second_try")
                    .WithSchedule(new RunOnceSchedule(sunEvents.SunsetUtc.AddMinutes(10)))
                    .WithScheduledTask((IEventBus eventBus) => eventBus.PublishAsync(new SunsetEvent(location.Id)));
                });
            }
        }
    }
}