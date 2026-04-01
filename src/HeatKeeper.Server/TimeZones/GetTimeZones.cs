namespace HeatKeeper.Server.TimeZones;

[RequireUserRole]
[Get("api/time-zones")]
public record GetTimeZonesQuery : IQuery<TimeZoneInfo[]>;

public record TimeZoneInfo(string Id, string DisplayName);

public class GetTimeZonesQueryHandler : IQueryHandler<GetTimeZonesQuery, TimeZoneInfo[]>
{
    public Task<TimeZoneInfo[]> HandleAsync(GetTimeZonesQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(System.TimeZoneInfo.GetSystemTimeZones()
            .Select(tz => new TimeZoneInfo(tz.Id, tz.DisplayName))
            .ToArray());
}
