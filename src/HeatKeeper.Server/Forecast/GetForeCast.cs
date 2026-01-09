using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Yr;
using Microsoft.Extensions.Logging;


[RequireUserRole]
[Get("api/locations/{LocationId}/forecast")]
public record GetForeCastQuery(long LocationId) : IQuery<ForeCast[]>;

public record ForeCast(DateOnly utcDate, DateTime utcSunRise, DateTime utcSunSet, TimeSeriesItem[] TimeSeries);

public record TimeSeriesItem(DateOnly Date, int Hour, double? Temperature, double? WindSpeed, double? WindSpeedOfGust, double? WindFromDirection, double PrecipitationAmount, double? PrecipitationAmountMin, double? PrecipitationAmountMax, string SymbolCode);

public class GetForeCastQueryHandler(IQueryExecutor queryExecutor, ILogger<GetForeCastQueryHandler> logger) : IQueryHandler<GetForeCastQuery, ForeCast[]>
{
    public async Task<ForeCast[]> HandleAsync(GetForeCastQuery query, CancellationToken cancellationToken = default)
    {
        var locationDetails = await queryExecutor.ExecuteAsync(new GetLocationDetailsQuery(query.LocationId), cancellationToken);

        var locationForecast = await queryExecutor.ExecuteAsync(new GetLocationForecastQuery((double)locationDetails.Latitude, (double)locationDetails.Longitude), cancellationToken);
        var series = locationForecast.Properties.TimeSeries.Where(ts => ts.Data.Next1Hours is not null).Select(ts => new TimeSeriesItem(
            Date: DateOnly.FromDateTime(ts.Time.DateTime),
            Hour: ts.Time.Hour,
            Temperature: ts.Data.Instant.Details.AirTemperature,
            WindSpeed: ts.Data.Instant.Details.WindSpeed,
            WindSpeedOfGust: ts.Data.Instant.Details.WindSpeedOfGust,
            WindFromDirection: ts.Data.Instant.Details.WindFromDirection,
            PrecipitationAmount: ts.Data.Next1Hours.Details.PrecipitationAmount,
            PrecipitationAmountMin: ts.Data.Next1Hours.Details.PrecipitationAmountMin,
            PrecipitationAmountMax: ts.Data.Next1Hours.Details.PrecipitationAmountMax,
            SymbolCode: ts.Data.Next1Hours.Summary.SymbolCode
        )).ToArray();


        var forecasts = series.GroupBy(t => t.Date).Select(g =>
        {
            var sunEvents = GetSunEventsForDate(g.Key, (double)locationDetails.Latitude, (double)locationDetails.Longitude);
            return new ForeCast(
                utcDate: g.Key,
                utcSunRise: sunEvents.Result.SunriseUtc,
                utcSunSet: sunEvents.Result.SunsetUtc,
                TimeSeries: g.ToArray());
        }).ToArray();

        return forecasts.OrderBy(f => f.utcDate).ToArray();
    }

    private async Task<SunEvents> GetSunEventsForDate(DateOnly utcDate, double latitude, double longitude)
    {
        var sunEvent = await queryExecutor.ExecuteAsync(new GetSunEventsQuery(latitude, longitude, utcDate));
        return sunEvent;
    }
}