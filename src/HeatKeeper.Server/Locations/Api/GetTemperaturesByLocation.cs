using System.ComponentModel.DataAnnotations;
namespace HeatKeeper.Server.Locations;

[RequireUserRole]
[Get("api/locations/{locationId}/temperatures")]
public record LocationTemperaturesQuery(long LocationId) : IQuery<LocationTemperature[]>;

public class GetTemperaturesByLocation(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<LocationTemperaturesQuery, LocationTemperature[]>
{
    public async Task<LocationTemperature[]> HandleAsync(LocationTemperaturesQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<LocationTemperature>(sqlProvider.GetDashboardTemperatures, query)).ToArray();
}

public record LocationTemperature(
    [property: Key] long ZoneId,
    string Name,
    double? Temperature,
    double? Humidity,
    DateTime Updated
    );