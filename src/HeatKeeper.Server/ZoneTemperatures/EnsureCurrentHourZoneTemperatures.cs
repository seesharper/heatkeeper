using HeatKeeper.Server.Measurements;

namespace HeatKeeper.Server.ZoneTemperatures;

[RequireBackgroundRole]
public record EnsureCurrentHourZoneTemperaturesCommand();

public class EnsureCurrentHourZoneTemperaturesCommandHandler(
    TimeProvider timeProvider,
    IDbConnection dbConnection,
    ISqlProvider sqlProvider) : ICommandHandler<EnsureCurrentHourZoneTemperaturesCommand>
{
    public async Task HandleAsync(EnsureCurrentHourZoneTemperaturesCommand command, CancellationToken cancellationToken = default)
    {
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        var currentHour = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, 0, 0, DateTimeKind.Utc);

        await dbConnection.ExecuteAsync(
            sqlProvider.InsertMissingZoneTemperatureForCurrentHour,
            new { CurrentHour = currentHour, MeasurementType = MeasurementType.Temperature });
    }
}
